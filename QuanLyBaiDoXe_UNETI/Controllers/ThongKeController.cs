using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI.Models;
using ClosedXML.Excel;
using System.IO;

namespace QuanLyBaiDoXe_UNETI.Controllers
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Mã sinh viên: 22203100007
    // Nội dung thực hiện: Module 4 - Thống kê báo cáo, Trực quan hóa số liệu Dashboard & Xuất file Excel (ClosedXML)
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Xác thực quyền Quản trị viên từ Session trước khi truy cập trang báo cáo
        /// </summary>
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var model = new DashboardViewModel();

            // Tổng hợp các chỉ số hoạt động tổng quan của bãi xe
            model.TongSoLoaiXe = await _context.LoaiPhuongTien.CountAsync();
            model.TongSoViTri = await _context.ViTriDoXe.CountAsync();
            model.SoViTriTrong = await _context.ViTriDoXe.CountAsync(v => v.TrangThai == "Còn trống");
            model.SoViTriDangDung = await _context.ViTriDoXe.CountAsync(v => v.TrangThai == "Đang sử dụng");
            model.TongSoChuXe = await _context.ChuPhuongTien.CountAsync();
            model.TongSoXe = await _context.PhuongTien.CountAsync();
            model.TongSoPhieu = await _context.PhieuGuiXe.CountAsync();
            model.SoPhieuChoXacNhan = await _context.PhieuGuiXe.CountAsync(p => p.TrangThai == "Chờ xác nhận");
            model.SoXeDangGui = await _context.PhieuGuiXe.CountAsync(p => p.TrangThai == "Đang gửi");

            // Tính tổng doanh thu thực tế từ toàn bộ các lượt gửi đã hoàn tất thanh toán
            model.TongDoanhThu = await _context.PhieuGuiXe
                .Where(p => p.TrangThai == "Đã lấy xe" && p.ThanhTien.HasValue)
                .SumAsync(p => p.ThanhTien!.Value);

            var dsLoaiXe = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachPhuongTien)
                .ToListAsync();

            var dsPhieuHoanTat = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Where(p => p.TrangThai == "Đã lấy xe")
                .ToListAsync();

            // Thống kê phân loại cơ cấu lượt gửi và doanh thu theo từng chủng loại phương tiện
            model.ThongKeTheoLoaiXe = dsLoaiXe.Select(l => new ThongKeLoaiXeItem
            {
                TenLoaiXe = l.TenLoaiPhuongTien,
                SoLuongXe = l.DanhSachPhuongTien.Count,
                SoLuotGui = dsPhieuHoanTat.Count(p => p.PhuongTien?.MaLoaiPhuongTien == l.MaLoaiPhuongTien),
                DoanhThu = dsPhieuHoanTat.Where(p => p.PhuongTien?.MaLoaiPhuongTien == l.MaLoaiPhuongTien).Sum(p => p.ThanhTien ?? 0)
            }).ToList();

            // Truy vấn tần suất sử dụng các vị trí đỗ và trích xuất Top 5 vị trí phát sinh lượt đỗ nhiều nhất
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
                .Take(5)
                .ToListAsync();

            // Gom nhóm và phân tích xu hướng biến động doanh thu theo từng chu kỳ Năm/Tháng
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

        [HttpGet]
        public async Task<IActionResult> XuatBaoCaoExcel()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

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

                worksheet.Cell("A1").Value = "BÁO CÁO DOANH THU BÃI ĐỖ XE SMARTPARKING UNETI";
                worksheet.Range("A1:G1").Merge().Style.Font.SetBold().Font.SetFontSize(16).Font.SetFontColor(XLColor.DarkBlue);
                worksheet.Range("A1:G1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell("A2").Value = $"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Range("A2:G2").Merge().Style.Font.SetItalic().Font.SetFontSize(11).Font.SetFontColor(XLColor.DimGray);
                worksheet.Range("A2:G2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                var headers = new string[] { "Mã Phiếu", "Biển Số Xe", "Chủ Xe", "Loại Phương Tiện", "Vị Trí Đỗ", "Thời Gian Ra", "Thành Tiền (VNĐ)" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(4, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.SetBold();
                    cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(14, 165, 233));
                    cell.Style.Font.SetFontColor(XLColor.White);
                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }

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

                // Thiết lập công thức hàm SUM tự động tính tổng tiền cột doanh thu
                worksheet.Cell(row, 6).Value = "TỔNG CỘNG:";
                worksheet.Cell(row, 6).Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                var cellTong = worksheet.Cell(row, 7);
                cellTong.FormulaA1 = $"SUM(G5:G{row - 1})";
                cellTong.Style.Font.SetBold().Font.SetFontColor(XLColor.DarkGreen);
                cellTong.Style.NumberFormat.Format = "#,##0";

                worksheet.Range(4, 1, row, 7).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(4, 1, row, 7).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                worksheet.Columns().AdjustToContents();

                // Xuất file trực tiếp vào bộ nhớ đệm (MemoryStream) và trả về định dạng OpenXML (.xlsx)
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