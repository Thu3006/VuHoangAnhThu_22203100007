using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI.Models;

namespace QuanLyBaiDoXe_UNETI.Controllers
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Mã sinh viên: 22203100007
    // Nội dung thực hiện:  Quản lý tài khoản, Xác thực người dùng, Phân quyền hệ thống & Bảo mật mật khẩu
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.TaiKhoan
                .FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap);

            // Xác thực thông tin đăng nhập bằng giải thuật băm mật khẩu
            if (user == null || !PasswordHelper.VerifyPassword(model.MatKhau, user.MatKhau))
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View(model);
            }

            // Cơ chế tương thích ngược: Tự động mã hóa sang chuẩn SHA-256 đối với các tài khoản dữ liệu cũ
            if (user.MatKhau == model.MatKhau)
            {
                user.MatKhau = PasswordHelper.HashPassword(model.MatKhau);
                await _context.SaveChangesAsync();
            }

            if (!user.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            // Khởi tạo phiên làm việc (Session) lưu trữ ngữ cảnh người dùng
            HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);

            if (user.VaiTro == "Admin")
            {
                return RedirectToAction("Index", "ThongKe");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuenMatKhau(string tenDangNhap, string email)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(email))
            {
                TempData["ForgotError"] = "Vui lòng nhập đầy đủ tên đăng nhập và email!";
                return RedirectToAction(nameof(DangNhap));
            }

            var user = await _context.TaiKhoan
                .FirstOrDefaultAsync(u => u.TenDangNhap.Trim().ToLower() == tenDangNhap.Trim().ToLower()
                                       && u.Email.Trim().ToLower() == email.Trim().ToLower());

            if (user == null)
            {
                TempData["ForgotError"] = "Thông tin tài khoản hoặc email không chính xác trong hệ thống!";
                return RedirectToAction(nameof(DangNhap));
            }

            // Khôi phục mật khẩu mặc định và lập tức băm bảo mật trước khi cập nhật CSDL
            user.MatKhau = PasswordHelper.HashPassword("123456");
            await _context.SaveChangesAsync();

            TempData["ForgotSuccess"] = $"Mật khẩu cho tài khoản [{user.TenDangNhap}] đã được khôi phục về mặc định: 123456. Vui lòng đăng nhập lại!";
            return RedirectToAction(nameof(DangNhap));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> HoSo()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction(nameof(DangNhap));
            }

            var taiKhoan = await _context.TaiKhoan.FindAsync(maTaiKhoan);
            if (taiKhoan == null) return RedirectToAction(nameof(DangNhap));

            var chuXe = await _context.ChuPhuongTien.FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);
            ViewBag.ChuXe = chuXe;

            return View(taiKhoan);
        }

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            {
                return RedirectToAction(nameof(DangNhap));
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return RedirectToAction(nameof(DangNhap));

            if (!ModelState.IsValid) return View(model);

            var user = await _context.TaiKhoan.FindAsync(maTaiKhoan);
            if (user == null) return RedirectToAction(nameof(DangNhap));

            // Xác thực tính hợp lệ của mật khẩu hiện tại thông qua mã băm lưu trữ
            if (!PasswordHelper.VerifyPassword(model.MatKhauCu, user.MatKhau))
            {
                ModelState.AddModelError("MatKhauCu", "Mật khẩu hiện tại không chính xác.");
                return View(model);
            }

            user.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(HoSo));
        }
    }
}