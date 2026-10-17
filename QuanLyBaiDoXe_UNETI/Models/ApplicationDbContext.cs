using Microsoft.EntityFrameworkCore;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TaiKhoan> TaiKhoan { get; set; }
        public DbSet<ChuPhuongTien> ChuPhuongTien { get; set; }
        public DbSet<LoaiPhuongTien> LoaiPhuongTien { get; set; }
        public DbSet<ViTriDoXe> ViTriDoXe { get; set; }
        public DbSet<PhuongTien> PhuongTien { get; set; }
        public DbSet<PhieuGuiXe> PhieuGuiXe { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ràng buộc Unique không trùng lặp theo yêu cầu đề bài
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap).IsUnique();

            modelBuilder.Entity<LoaiPhuongTien>()
                .HasIndex(l => l.TenLoaiPhuongTien).IsUnique();

            modelBuilder.Entity<ViTriDoXe>()
                .HasIndex(v => v.TenViTri).IsUnique();

            modelBuilder.Entity<PhuongTien>()
                .HasIndex(p => p.BienSoXe).IsUnique();

            // Cấu hình quan hệ 1 - 1 giữa TaiKhoan và ChuPhuongTien
            modelBuilder.Entity<ChuPhuongTien>()
                .HasOne(c => c.TaiKhoan)
                .WithOne(t => t.ChuPhuongTien)
                .HasForeignKey<ChuPhuongTien>(c => c.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            // Ngăn chặn vòng lặp Cascade Delete khi xóa dữ liệu liên kết với PhieuGuiXe
            modelBuilder.Entity<PhieuGuiXe>()
                .HasOne(p => p.ViTriDoXe)
                .WithMany(v => v.DanhSachPhieuGui)
                .HasForeignKey(p => p.MaViTri)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhieuGuiXe>()
                .HasOne(p => p.PhuongTien)
                .WithMany(pt => pt.DanhSachPhieuGui)
                .HasForeignKey(p => p.MaPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}