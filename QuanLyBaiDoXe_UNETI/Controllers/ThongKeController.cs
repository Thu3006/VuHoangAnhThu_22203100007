using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI.Models;
using ClosedXML.Excel;
using System.IO;

namespace QuanLyBaiDoXe_UNETI.Controllers
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Nội dung: Dashboard và các truy vấn LINQ Thống kê phân tích dữ liệu
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        // GET: ThongKe (Dashboard & Báo cáo)
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var model = new DashboardViewModel();

            // 1. Thống kê số lượng tổng quan bằng LINQ Count / Sum
            model.TongSoLoaiXe = await _context.LoaiPhuongTien.CountAsync();
            model.TongSoViTri = await _context.ViTriDoXe.CountAsync();
            model.SoViTriTrong = await _context.ViTriDoXe.CountAsync(v => v.TrangThai == "Còn trống");
            model.SoViTriDangDung = await _context.ViTriDoXe.CountAsync(v => v.TrangThai == "Đang sử dụng");
            model.TongSoChuXe = await _context.ChuPhuongTien.CountAsync();
            model.TongSoXe = await _context.PhuongTien.CountAsync();
            model.TongSoPhieu = await _context.PhieuGuiXe.CountAsync();
            model.SoPhieuChoXacNhan = await _context.PhieuGuiXe.CountAsync(p => p.TrangThai == "Chờ xác nhận");
            model.SoXeDangGui = await _context.PhieuGuiXe.CountAsync(p => p.TrangThai == "Đang gửi");

            // Tổng doanh thu từ các phiếu đã hoàn thành (Đã lấy xe)
            model.TongDoanhThu = await _context.PhieuGuiXe
                .Where(p => p.TrangThai == "Đã lấy xe" && p.ThanhTien.HasValue)
                .SumAsync(p => p.ThanhTien!.Value);

            // 2. Thống kê theo từng loại xe (LINQ GroupBy)
            var dsLoaiXe = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachPhuongTien)
                .ToListAsync();

            var dsPhieuHoanTat = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Where(p => p.TrangThai == "Đã lấy xe")
                .ToListAsync();

            model.ThongKeTheoLoaiXe = dsLoaiXe.Select(l => new ThongKeLoaiXeItem
            {
                TenLoaiXe = l.TenLoaiPhuongTien,
                SoLuongXe = l.DanhSachPhuongTien.Count,
                SoLuotGui = dsPhieuHoanTat.Count(p => p.PhuongTien?.MaLoaiPhuongTien == l.MaLoaiPhuongTien),
                DoanhThu = dsPhieuHoanTat.Where(p => p.PhuongTien?.MaLoaiPhuongTien == l.MaLoaiPhuongTien).Sum(p => p.ThanhTien ?? 0)
            }).ToList();

            // 3. Vị trí đỗ có số lượt sử dụng nhiều nhất (LINQ GroupBy + OrderByDescending)
            model.ThongKeTheoViTri = await _context.PhieuGuiXe
                .Include(p => p.ViTriDoXe)
                .Where(p => p.TrangThai != "Đã hủy" && p.ViTriDoXe != null)
                .GroupBy(p => new { p.MaViTri, p.ViTriDoXe!.TenViTri, p.ViTriDoXe.KhuVuc, p.ViTriDoXe.Tang })
                .Select(g => new ThongKeViTriItem
                {
                    TenViTri = g.Key.TenViTri,
                    KhuVuc = g.Key.KhuVuc,
                    Tang = g.Key.Tang,
                    SoLuotSuDung = g.Count()
                })
                .OrderByDescending(v => v.SoLuotSuDung)
                .Take(5) // Lấy Top 5 vị trí được đỗ nhiều nhất
                .ToListAsync();

            // 4. Doanh thu và lượt gửi theo từng tháng (LINQ GroupBy theo Năm/Tháng)
            model.DoanhThuTheoThang = await _context.PhieuGuiXe
                .Where(p => p.TrangThai == "Đã lấy xe" && p.ThoiGianRaThucTe.HasValue)
                .GroupBy(p => new { Nam = p.ThoiGianRaThucTe!.Value.Year, Thang = p.ThoiGianRaThucTe.Value.Month })
                .Select(g => new DoanhThuThangItem
                {
                    Nam = g.Key.Nam,
                    Thang = g.Key.Thang,
                    SoLuotGui = g.Count(),
                    TongTien = g.Sum(p => p.ThanhTien ?? 0)
                })
                .OrderByDescending(d => d.Nam)
                .ThenByDescending(d => d.Thang)
                .ToListAsync();

            return View(model);
        }
        // GET: /ThongKe/XuatBaoCaoExcel
        [HttpGet]
        public async Task<IActionResult> XuatBaoCaoExcel()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy danh sách các phiếu gửi xe đã hoàn thành (Đã lấy xe)
            var danhSachPhieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                    .ThenInclude(pt => pt!.LoaiPhuongTien)
                .Include(p => p.PhuongTien)
                    .ThenInclude(pt => pt!.ChuPhuongTien)
                .Include(p => p.ViTriDoXe)
                .Where(p => p.TrangThai == "Đã lấy xe")
                .OrderByDescending(p => p.ThoiGianRaThucTe)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("BaoCaoDoanhThu");

                // 1. Tiêu đề báo cáo
                worksheet.Cell("A1").Value = "BÁO CÁO DOANH THU BÃI ĐỖ XE SMARTPARKING UNETI";
                worksheet.Range("A1:G1").Merge().Style.Font.SetBold().Font.SetFontSize(16).Font.SetFontColor(XLColor.DarkBlue);
                worksheet.Range("A1:G1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell("A2").Value = $"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Range("A2:G2").Merge().Style.Font.SetItalic().Font.SetFontSize(11).Font.SetFontColor(XLColor.DimGray);
                worksheet.Range("A2:G2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // 2. Tiêu đề các cột
                var headers = new string[] { "Mã Phiếu", "Biển Số Xe", "Chủ Xe", "Loại Phương Tiện", "Vị Trí Đỗ", "Thời Gian Ra", "Thành Tiền (VNĐ)" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(4, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.SetBold();
                    cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(14, 165, 233)); // Màu xanh chủ đạo của app
                    cell.Style.Font.SetFontColor(XLColor.White);
                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }

                // 3. Đổ dữ liệu từng dòng
                int row = 5;
                foreach (var item in danhSachPhieu)
                {
                    worksheet.Cell(row, 1).Value = item.MaPhieu;
                    worksheet.Cell(row, 2).Value = item.PhuongTien?.BienSoXe ?? "—";
                    worksheet.Cell(row, 3).Value = item.PhuongTien?.ChuPhuongTien?.HoTen ?? "Khách vãng lai";
                    worksheet.Cell(row, 4).Value = item.PhuongTien?.LoaiPhuongTien?.TenLoaiPhuongTien ?? "—";
                    worksheet.Cell(row, 5).Value = item.ViTriDoXe?.TenViTri ?? "—";
                    worksheet.Cell(row, 6).Value = item.ThoiGianRaThucTe.HasValue ? item.ThoiGianRaThucTe.Value.ToString("dd/MM/yyyy HH:mm") : "—";

                    var cellTien = worksheet.Cell(row, 7);
                    cellTien.Value = item.ThanhTien ?? 0;
                    cellTien.Style.NumberFormat.Format = "#,##0";

                    row++;
                }

                // 4. Dòng tổng cộng doanh thu
                worksheet.Cell(row, 6).Value = "TỔNG CỘNG:";
                worksheet.Cell(row, 6).Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                var cellTong = worksheet.Cell(row, 7);
                cellTong.FormulaA1 = $"SUM(G5:G{row - 1})";
                cellTong.Style.Font.SetBold().Font.SetFontColor(XLColor.DarkGreen);
                cellTong.Style.NumberFormat.Format = "#,##0";

                // Kẻ viền (border) và tự căn chỉnh độ rộng cột
                worksheet.Range(4, 1, row, 7).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(4, 1, row, 7).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var fileName = $"BaoCao_DoanhThu_SmartParking_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

    }
}