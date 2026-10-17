using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI.Models;

namespace QuanLyBaiDoXe_UNETI.Controllers
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Nội dung: Module 3 - Quản lý xe cá nhân, Đăng ký gửi xe
    public class PhuongTienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhuongTienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy thông tin chủ phương tiện tương ứng với tài khoản đang đăng nhập
        private async Task<ChuPhuongTien?> GetCurrentChuXeAsync()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;

            return await _context.ChuPhuongTien.FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);
        }

        // GET: PhuongTien (Danh sách xe của tôi)
        public async Task<IActionResult> Index()
        {
            var chuXe = await GetCurrentChuXeAsync();
            if (chuXe == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var danhSachXe = await _context.PhuongTien
                .Include(p => p.LoaiPhuongTien)
                .Where(p => p.MaChuPhuongTien == chuXe.MaChuPhuongTien)
                .ToListAsync();

            return View(danhSachXe);
        }

        // GET: PhuongTien/Create
        public async Task<IActionResult> Create()
        {
            var chuXe = await GetCurrentChuXeAsync();
            if (chuXe == null) return RedirectToAction("DangNhap", "TaiKhoan");

            ViewBag.MaLoaiPhuongTien = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien");
            return View();
        }

        // POST: PhuongTien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhuongTien phuongTien)
        {
            var chuXe = await GetCurrentChuXeAsync();
            if (chuXe == null) return RedirectToAction("DangNhap", "TaiKhoan");

            // Ràng buộc: Biển số xe không được trùng lặp
            bool isExist = await _context.PhuongTien.AnyAsync(p => p.BienSoXe.Trim().ToLower() == phuongTien.BienSoXe.Trim().ToLower());
            if (isExist)
            {
                ModelState.AddModelError("BienSoXe", "Biển số xe này đã được đăng ký trên hệ thống.");
            }

            phuongTien.MaChuPhuongTien = chuXe.MaChuPhuongTien;
            phuongTien.TrangThai = true;

            if (ModelState.IsValid)
            {
                _context.Add(phuongTien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng ký phương tiện mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoaiPhuongTien = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", phuongTien.MaLoaiPhuongTien);
            return View(phuongTien);
        }

        // GET: PhuongTien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var chuXe = await GetCurrentChuXeAsync();
            if (chuXe == null) return RedirectToAction("DangNhap", "TaiKhoan");
            if (id == null) return NotFound();

            // Phân quyền dữ liệu: Không cho sửa xe của người khác
            var xe = await _context.PhuongTien.FirstOrDefaultAsync(p => p.MaPhuongTien == id && p.MaChuPhuongTien == chuXe.MaChuPhuongTien);
            if (xe == null) return NotFound();

            ViewBag.MaLoaiPhuongTien = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", xe.MaLoaiPhuongTien);
            return View(xe);
        }

        // POST: PhuongTien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhuongTien phuongTien)
        {
            var chuXe = await GetCurrentChuXeAsync();
            if (chuXe == null) return RedirectToAction("DangNhap", "TaiKhoan");
            if (id != phuongTien.MaPhuongTien) return NotFound();

            // Kiểm tra biển số trùng với xe khác
            bool isDuplicate = await _context.PhuongTien.AnyAsync(p => p.MaPhuongTien != id && p.BienSoXe.Trim().ToLower() == phuongTien.BienSoXe.Trim().ToLower());
            if (isDuplicate)
            {
                ModelState.AddModelError("BienSoXe", "Biển số xe này đã được sử dụng bởi phương tiện khác.");
            }

            phuongTien.MaChuPhuongTien = chuXe.MaChuPhuongTien;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phuongTien);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật phương tiện thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PhuongTien.Any(e => e.MaPhuongTien == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoaiPhuongTien = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", phuongTien.MaLoaiPhuongTien);
            return View(phuongTien);
        }
    }
}