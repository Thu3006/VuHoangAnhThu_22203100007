using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    [Table("PhuongTien")]
    public class PhuongTien
    {
        [Key]
        public int MaPhuongTien { get; set; }

        [Required]
        public int MaChuPhuongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại phương tiện")]
        public int MaLoaiPhuongTien { get; set; }

        [Required(ErrorMessage = "Biển số xe không được để trống")]
        [StringLength(20)]
        public string BienSoXe { get; set; } = string.Empty;

        [StringLength(50)]
        public string? NhanHieu { get; set; }

        [StringLength(30)]
        public string? MauSac { get; set; }

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        // Navigation Properties
        [ForeignKey("MaChuPhuongTien")]
        public virtual ChuPhuongTien? ChuPhuongTien { get; set; }

        [ForeignKey("MaLoaiPhuongTien")]
        public virtual LoaiPhuongTien? LoaiPhuongTien { get; set; }

        public virtual ICollection<PhieuGuiXe> DanhSachPhieuGui { get; set; } = new List<PhieuGuiXe>();
    }
}