using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI.Models;

namespace QuanLyBaiDoXe_UNETI.Controllers
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Mã sinh viên: 22203100007
    // Nội dung thực hiện: Module 1 - Quản lý loại phương tiện (CRUD, Validation, Phân quyền)
    public class LoaiPhuongTienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiPhuongTienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm tiện ích: Kiểm tra quyền Admin từ Session
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("VaiTro");
            return role == "Admin";
        }

        // GET: LoaiPhuongTien
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var danhSach = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachViTri)
                .Include(l => l.DanhSachPhuongTien)
                .ToListAsync();

            return View(danhSach);
        }

        // GET: LoaiPhuongTien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            if (id == null) return NotFound();

            var loai = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachViTri)
                .Include(l => l.DanhSachPhuongTien)
                .FirstOrDefaultAsync(m => m.MaLoaiPhuongTien == id);

            if (loai == null) return NotFound();

            return View(loai);
        }

        // GET: LoaiPhuongTien/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            return View();
        }

        // POST: LoaiPhuongTien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiPhuongTien loaiPhuongTien)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            // Kiểm tra trùng tên loại phương tiện bằng LINQ
            bool isExist = await _context.LoaiPhuongTien
                .AnyAsync(l => l.TenLoaiPhuongTien.Trim().ToLower() == loaiPhuongTien.TenLoaiPhuongTien.Trim().ToLower());

            if (isExist)
            {
                ModelState.AddModelError("TenLoaiPhuongTien", "Tên loại phương tiện này đã tồn tại trong hệ thống.");
            }

            if (loaiPhuongTien.DonGiaTheoGio <= 0)
            {
                ModelState.AddModelError("DonGiaTheoGio", "Đơn giá theo giờ phải lớn hơn 0 VNĐ.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(loaiPhuongTien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới loại phương tiện thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(loaiPhuongTien);
        }

        // GET: LoaiPhuongTien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            if (id == null) return NotFound();

            var loai = await _context.LoaiPhuongTien.FindAsync(id);
            if (loai == null) return NotFound();

            return View(loai);
        }

        // POST: LoaiPhuongTien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiPhuongTien loaiPhuongTien)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            if (id != loaiPhuongTien.MaLoaiPhuongTien) return NotFound();

            // Kiểm tra trùng tên với bản ghi khác
            bool isDuplicate = await _context.LoaiPhuongTien
                .AnyAsync(l => l.MaLoaiPhuongTien != id && l.TenLoaiPhuongTien.Trim().ToLower() == loaiPhuongTien.TenLoaiPhuongTien.Trim().ToLower());

            if (isDuplicate)
            {
                ModelState.AddModelError("TenLoaiPhuongTien", "Tên loại phương tiện này đã được sử dụng.");
            }

            if (loaiPhuongTien.DonGiaTheoGio <= 0)
            {
                ModelState.AddModelError("DonGiaTheoGio", "Đơn giá theo giờ phải lớn hơn 0 VNĐ.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiPhuongTien);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật loại phương tiện thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LoaiPhuongTien.Any(e => e.MaLoaiPhuongTien == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loaiPhuongTien);
        }

        // POST: LoaiPhuongTien/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var loai = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachViTri)
                .Include(l => l.DanhSachPhuongTien)
                .FirstOrDefaultAsync(l => l.MaLoaiPhuongTien == id);

            if (loai == null) return NotFound();

            // Ràng buộc bắt buộc: Không cho xóa nếu đang có xe hoặc vị trí đỗ tham chiếu tới
            if (loai.DanhSachViTri.Any() || loai.DanhSachPhuongTien.Any())
            {
                TempData["ErrorMessage"] = $"Không thể xóa loại [{loai.TenLoaiPhuongTien}] vì đang có {loai.DanhSachViTri.Count} vị trí đỗ hoặc {loai.DanhSachPhuongTien.Count} phương tiện sử dụng!";
                return RedirectToAction(nameof(Index));
            }

            _context.LoaiPhuongTien.Remove(loai);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa loại phương tiện thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}