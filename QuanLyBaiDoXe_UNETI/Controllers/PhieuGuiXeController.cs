using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI.Models;

namespace QuanLyBaiDoXe_UNETI.Controllers
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Nội dung: Xử lý quy trình gửi xe, xác nhận vào/ra, tính phí tự động, quản lý phiếu
    public class PhieuGuiXeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuGuiXeController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        private async Task<ChuPhuongTien?> GetCurrentChuXeAsync()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;
            return await _context.ChuPhuongTien.FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);
        }

        // ========================== PHÍA KHÁCH HÀNG ==========================

        // GET: PhieuGuiXe/DangKy
        [HttpGet]
        public async Task<IActionResult> DangKy(int? maViTri)
        {
            var chuXe = await GetCurrentChuXeAsync();
            if (chuXe == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var danhSachXe = await _context.PhuongTien
                .Where(p => p.MaChuPhuongTien == chuXe.MaChuPhuongTien && p.TrangThai)
                .ToListAsync();

            if (!danhSachXe.Any())
            {
                TempData["ErrorMessage"] = "Bạn chưa có phương tiện nào hoạt động. Vui lòng thêm xe trước khi đăng ký gửi!";
                return RedirectToAction("Create", "PhuongTien");
            }

            ViewBag.MaPhuongTien = new SelectList(danhSachXe, "MaPhuongTien", "BienSoXe");
            ViewBag.MaViTri = new SelectList(await _context.ViTriDoXe.Where(v => v.TrangThai != "Tạm ngừng").ToListAsync(), "MaViTri", "TenViTri", maViTri);

            var model = new DangKyGuiXeViewModel
            {
                MaViTri = maViTri ?? 0,
                ThoiGianDuKienVao = DateTime.Now,
                ThoiGianDuKienRa = DateTime.Now.AddHours(3)
            };

            return View(model);
        }

        // POST: PhieuGuiXe/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyGuiXeViewModel model)
        {
            var chuXe = await GetCurrentChuXeAsync();
            if (chuXe == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var phuongTien = await _context.PhuongTien
                .Include(p => p.LoaiPhuongTien)
                .FirstOrDefaultAsync(p => p.MaPhuongTien == model.MaPhuongTien && p.MaChuPhuongTien == chuXe.MaChuPhuongTien);

            if (phuongTien == null)
            {
                ModelState.AddModelError("MaPhuongTien", "Phương tiện không hợp lệ hoặc không thuộc tài khoản của bạn.");
            }

            var viTri = await _context.ViTriDoXe.FindAsync(model.MaViTri);
            if (viTri == null || viTri.TrangThai == "Tạm ngừng")
            {
                ModelState.AddModelError("MaViTri", "Vị trí đỗ không tồn tại hoặc đang tạm ngừng phục vụ.");
            }
            else if (phuongTien != null && viTri.MaLoaiPhuongTien != phuongTien.MaLoaiPhuongTien)
            {
                ModelState.AddModelError("MaViTri", "Vị trí này không phù hợp với loại phương tiện của bạn.");
            }

            if (model.ThoiGianDuKienRa <= model.ThoiGianDuKienVao)
            {
                ModelState.AddModelError("ThoiGianDuKienRa", "Thời gian dự kiến ra phải sau thời gian dự kiến vào.");
            }

            bool xeDangGui = await _context.PhieuGuiXe.AnyAsync(p =>
                p.MaPhuongTien == model.MaPhuongTien &&
                (p.TrangThai == "Chờ xác nhận" || p.TrangThai == "Đang gửi"));

            if (xeDangGui)
            {
                ModelState.AddModelError("MaPhuongTien", "Phương tiện này hiện đang có một phiếu gửi xe chưa hoàn tất.");
            }

            bool biTrungLich = await _context.PhieuGuiXe.AnyAsync(p =>
                p.MaViTri == model.MaViTri &&
                p.TrangThai != "Đã hủy" && p.TrangThai != "Đã lấy xe" &&
                model.ThoiGianDuKienVao < p.ThoiGianDuKienRa &&
                model.ThoiGianDuKienRa > p.ThoiGianDuKienVao);

            if (biTrungLich)
            {
                ModelState.AddModelError("MaViTri", "Vị trí này đã được đặt trong khoảng thời gian trên. Vui lòng chọn vị trí hoặc khung giờ khác!");
            }

            if (ModelState.IsValid && phuongTien?.LoaiPhuongTien != null)
            {
                var phieuMoi = new PhieuGuiXe
                {
                    MaPhuongTien = model.MaPhuongTien,
                    MaViTri = model.MaViTri,
                    NgayDangKy = DateTime.Now,
                    ThoiGianDuKienVao = model.ThoiGianDuKienVao,
                    ThoiGianDuKienRa = model.ThoiGianDuKienRa,
                    DonGiaTheoGio = phuongTien.LoaiPhuongTien.DonGiaTheoGio,
                    TrangThai = "Chờ xác nhận"
                };

                _context.PhieuGuiXe.Add(phieuMoi);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký gửi xe thành công! Phiếu đang ở trạng thái Chờ xác nhận.";
                return RedirectToAction(nameof(LichSu));
            }

            ViewBag.MaPhuongTien = new SelectList(await _context.PhuongTien.Where(p => p.MaChuPhuongTien == chuXe.MaChuPhuongTien && p.TrangThai).ToListAsync(), "MaPhuongTien", "BienSoXe", model.MaPhuongTien);
            ViewBag.MaViTri = new SelectList(await _context.ViTriDoXe.Where(v => v.TrangThai != "Tạm ngừng").ToListAsync(), "MaViTri", "TenViTri", model.MaViTri);
            return View(model);
        }

        // GET: PhieuGuiXe/LichSu
        public async Task<IActionResult> LichSu()
        {
            var chuXe = await GetCurrentChuXeAsync();
            if (chuXe == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var danhSachPhieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .Where(p => p.PhuongTien!.MaChuPhuongTien == chuXe.MaChuPhuongTien)
                .OrderByDescending(p => p.NgayDangKy)
                .ToListAsync();

            return View(danhSachPhieu);
        }

        // ========================== PHÍA ADMIN ==========================

        // GET: PhieuGuiXe/Index (Admin quản lý danh sách phiếu, tìm kiếm & lọc)
        public async Task<IActionResult> Index(string? searchTerm, string? trangThai, int? maLoaiPhuongTien)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var query = _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                    .ThenInclude(pt => pt!.ChuPhuongTien)
                .Include(p => p.PhuongTien)
                    .ThenInclude(pt => pt!.LoaiPhuongTien)
                .Include(p => p.ViTriDoXe)
                .AsQueryable();

            // Tìm kiếm theo Biển số xe hoặc Tên chủ phương tiện bằng LINQ
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    (p.PhuongTien != null && p.PhuongTien.BienSoXe.ToLower().Contains(term)) ||
                    (p.PhuongTien != null && p.PhuongTien.ChuPhuongTien != null && p.PhuongTien.ChuPhuongTien.HoTen.ToLower().Contains(term)));
            }

            // Lọc theo trạng thái phiếu
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(p => p.TrangThai == trangThai);
            }

            // Lọc theo loại phương tiện
            if (maLoaiPhuongTien.HasValue && maLoaiPhuongTien.Value > 0)
            {
                query = query.Where(p => p.PhuongTien != null && p.PhuongTien.MaLoaiPhuongTien == maLoaiPhuongTien.Value);
            }

            var danhSach = await query.OrderByDescending(p => p.NgayDangKy).ToListAsync();

            ViewBag.DanhSachLoaiXe = new SelectList(await _context.LoaiPhuongTien.ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", maLoaiPhuongTien);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.TrangThai = trangThai;

            return View(danhSach);
        }

        // POST: PhieuGuiXe/XacNhanVao/5 (Xác nhận xe vào bãi)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanVao(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var phieu = await _context.PhieuGuiXe
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null || phieu.TrangThai != "Chờ xác nhận")
            {
                TempData["ErrorMessage"] = "Phiếu gửi không tồn tại hoặc trạng thái không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật trạng thái phiếu: Chờ xác nhận -> Đang gửi
            phieu.ThoiGianVaoThucTe = DateTime.Now;
            phieu.TrangThai = "Đang gửi";

            // Cập nhật vị trí đỗ: Đang sử dụng
            if (phieu.ViTriDoXe != null)
            {
                phieu.ViTriDoXe.TrangThai = "Đang sử dụng";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Xác nhận xe vào bãi thành công cho phiếu #{phieu.MaPhieu}!";
            return RedirectToAction(nameof(Index));
        }

        // POST: PhieuGuiXe/XacNhanRa/5 (Xác nhận xe ra bãi & Tính tiền tự động)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanRa(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var phieu = await _context.PhieuGuiXe
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null || phieu.TrangThai != "Đang gửi")
            {
                TempData["ErrorMessage"] = "Phiếu không hợp lệ hoặc xe chưa vào bãi!";
                return RedirectToAction(nameof(Index));
            }

            var thoiGianRa = DateTime.Now;
            var thoiGianVao = phieu.ThoiGianVaoThucTe ?? phieu.ThoiGianDuKienVao;

            if (thoiGianRa <= thoiGianVao)
            {
                thoiGianRa = thoiGianVao.AddMinutes(5); // Đảm bảo thời gian ra luôn sau thời gian vào
            }

            phieu.ThoiGianRaThucTe = thoiGianRa;

            // QUY TẮC TÍNH TIỀN TỰ ĐỘNG:
            // Tính số giờ thực tế, làm tròn lên theo mốc (tối thiểu 1 giờ, lẻ phút tính tròn 1 giờ)
            var tongGio = (thoiGianRa - thoiGianVao).TotalHours;
            int soGioTinhPhi = (int)Math.Ceiling(tongGio);
            if (soGioTinhPhi < 1) soGioTinhPhi = 1;

            // Công thức chuẩn: ThanhTien = SoGioTinhPhi * DonGiaTheoGio
            phieu.ThanhTien = soGioTinhPhi * phieu.DonGiaTheoGio;
            phieu.TrangThai = "Đã lấy xe";

            // Giải phóng vị trí đỗ về: Còn trống
            if (phieu.ViTriDoXe != null)
            {
                phieu.ViTriDoXe.TrangThai = "Còn trống";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Xác nhận xe ra thành công! Số giờ: {soGioTinhPhi}h - Thành tiền: {phieu.ThanhTien:N0} VNĐ";
            return RedirectToAction(nameof(Index));
        }

        // POST: PhieuGuiXe/HuyPhieu/5 (Hủy phiếu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyPhieu(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var phieu = await _context.PhieuGuiXe.FindAsync(id);
            if (phieu == null || phieu.TrangThai != "Chờ xác nhận")
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy những phiếu đang ở trạng thái Chờ xác nhận!";
                return RedirectToAction(nameof(Index));
            }

            phieu.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã hủy phiếu #{phieu.MaPhieu} thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}