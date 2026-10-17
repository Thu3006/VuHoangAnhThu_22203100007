using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    [Table("LoaiPhuongTien")]
    public class LoaiPhuongTien
    {
        [Key]
        public int MaLoaiPhuongTien { get; set; }

        [Required(ErrorMessage = "Tên loại phương tiện không được để trống")]
        [StringLength(50)]
        public string TenLoaiPhuongTien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đơn giá theo giờ bắt buộc")]
        [Range(1000, 10000000, ErrorMessage = "Đơn giá theo giờ phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaTheoGio { get; set; }

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        // Quan hệ 1 - n
        public virtual ICollection<ViTriDoXe> DanhSachViTri { get; set; } = new List<ViTriDoXe>();
        public virtual ICollection<PhuongTien> DanhSachPhuongTien { get; set; } = new List<PhuongTien>();
    }
}