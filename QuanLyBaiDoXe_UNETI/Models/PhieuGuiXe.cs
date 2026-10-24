using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    [Table("PhieuGuiXe")]
    public class PhieuGuiXe
    {
        [Key]
        public int MaPhieu { get; set; }

        [Required]
        public int MaPhuongTien { get; set; }

        [Required]
        public int MaViTri { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Thời gian dự kiến vào là bắt buộc")]
        public DateTime ThoiGianDuKienVao { get; set; }

        [Required(ErrorMessage = "Thời gian dự kiến ra là bắt buộc")]
        public DateTime ThoiGianDuKienRa { get; set; }

        public DateTime? ThoiGianVaoThucTe { get; set; }
        public DateTime? ThoiGianRaThucTe { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaTheoGio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ThanhTien { get; set; }

        [Required]
        [StringLength(30)]
        public string TrangThai { get; set; } = "Chờ xác nhận"; 

        [ForeignKey("MaPhuongTien")]
        public virtual PhuongTien? PhuongTien { get; set; }

        [ForeignKey("MaViTri")]
        public virtual ViTriDoXe? ViTriDoXe { get; set; }
    }
}