using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    [Table("ChuPhuongTien")]
    public class ChuPhuongTien
    {
        [Key]
        public int MaChuPhuongTien { get; set; }

        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
        public virtual ICollection<PhuongTien> DanhSachPhuongTien { get; set; } = new List<PhuongTien>();
    }
}