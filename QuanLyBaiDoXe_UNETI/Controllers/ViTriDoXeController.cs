using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI.Models;

namespace QuanLyBaiDoXe_UNETI.Controllers
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Nội dung: Module 2 - Quản lý vị trí đỗ, Tìm kiếm, Lọc, Sắp xếp, Phân trang LINQ
    public class ViTriDoXeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViTriDoXeController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        // GET: ViTriDoXe (Áp dụng: Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang -> Hiển thị)
        public async Task<IActionResult> Index(
            string? searchTerm,
            int? maLoaiPhuongTien,
            string? khuVuc,
            int? tang,
            string? trangThai,
            string? sortOrder,
            int page = 1)
        {
            // 1. Khởi tạo truy vấn kèm thông tin Loại xe liên kết
            var query = _context.ViTriDoXe
                .Include(v => v.LoaiPhuongTien)
                .AsQueryable();

            // 2. Tìm kiếm theo Tên vị trí hoặc Khu vực bằng LINQ
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(v => v.TenViTri.ToLower().Contains(term) || v.KhuVuc.ToLower().Contains(term));
            }

            // 3. Lọc đa điều kiện kết hợp
            if (maLoaiPhuongTien.HasValue && maLoaiPhuongTien.Value > 0)
            {
                query = query.Where(v => v.MaLoaiPhuongTien == maLoaiPhuongTien.Value);
            }

            if (!string.IsNullOrWhiteSpace(khuVuc))
            {
                query = query.Where(v => v.KhuVuc == khuVuc);
            }

            if (tang.HasValue && tang.Value > 0)
            {
                query = query.Where(v => v.Tang == tang.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(v => v.TrangThai == trangThai);
            }

            // 4. Sắp xếp theo yêu cầu
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(v => v.TenViTri),
                "tang_asc" => query.OrderBy(v => v.Tang).ThenBy(v => v.TenViTri),
                "tang_desc" => query.OrderByDescending(v => v.Tang).ThenBy(v => v.TenViTri),
                _ => query.OrderBy(v => v.TenViTri) // Mặc định: Tên vị trí A -> Z
            };

            // 5. Phân trang trên LINQ bằng Skip và Take
            int pageSize = 8;
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

            var danhSach = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Chuẩn bị dữ liệu cho các Dropdown List lọc
            ViewBag.DanhSachLoaiXe = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", maLoaiPhuongTien);
            ViewBag.DanhSachKhuVuc = await _context.ViTriDoXe.Select(v => v.KhuVuc).Distinct().ToListAsync();
            ViewBag.DanhSachTang = await _context.ViTriDoXe.Select(v => v.Tang).Distinct().OrderBy(t => t).ToListAsync();

            var viewModel = new ViTriDoXeIndexViewModel
            {
                DanhSachViTri = danhSach,
                SearchTerm = searchTerm,
                MaLoaiPhuongTien = maLoaiPhuongTien,
                KhuVuc = khuVuc,
                Tang = tang,
                TrangThai = trangThai,
                SortOrder = sortOrder,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems
            };

            return View(viewModel);
        }

        // GET: ViTriDoXe/Create (Admin)
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            ViewBag.MaLoaiPhuongTien = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien");
            return View();
        }

        // POST: ViTriDoXe/Create (Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ViTriDoXe viTriDoXe)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            bool isExist = await _context.ViTriDoXe.AnyAsync(v => v.TenViTri.Trim().ToLower() == viTriDoXe.TenViTri.Trim().ToLower());
            if (isExist)
            {
                ModelState.AddModelError("TenViTri", "Tên vị trí đỗ xe này đã tồn tại trong hệ thống.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(viTriDoXe);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới vị trí đỗ xe thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoaiPhuongTien = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", viTriDoXe.MaLoaiPhuongTien);
            return View(viTriDoXe);
        }

        // GET: ViTriDoXe/Edit/5 (Admin)
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            if (id == null) return NotFound();

            var viTri = await _context.ViTriDoXe.FindAsync(id);
            if (viTri == null) return NotFound();

            ViewBag.MaLoaiPhuongTien = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", viTri.MaLoaiPhuongTien);
            return View(viTri);
        }

        // POST: ViTriDoXe/Edit/5 (Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ViTriDoXe viTriDoXe)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            if (id != viTriDoXe.MaViTri) return NotFound();

            bool isDuplicate = await _context.ViTriDoXe.AnyAsync(v => v.MaViTri != id && v.TenViTri.Trim().ToLower() == viTriDoXe.TenViTri.Trim().ToLower());
            if (isDuplicate)
            {
                ModelState.AddModelError("TenViTri", "Tên vị trí đỗ này đã được sử dụng.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viTriDoXe);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật vị trí đỗ thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ViTriDoXe.Any(e => e.MaViTri == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoaiPhuongTien = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", viTriDoXe.MaLoaiPhuongTien);
            return View(viTriDoXe);
        }

        // POST: ViTriDoXe/Delete/5 (Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var viTri = await _context.ViTriDoXe
                .Include(v => v.DanhSachPhieuGui)
                .FirstOrDefaultAsync(v => v.MaViTri == id);

            if (viTri == null) return NotFound();

            if (viTri.DanhSachPhieuGui.Any(p => p.TrangThai == "Đang gửi" || p.TrangThai == "Chờ xác nhận"))
            {
                TempData["ErrorMessage"] = $"Không thể xóa vị trí [{viTri.TenViTri}] vì đang có lượt gửi xe hoạt động!";
                return RedirectToAction(nameof(Index));
            }

            _context.ViTriDoXe.Remove(viTri);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa vị trí đỗ thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}