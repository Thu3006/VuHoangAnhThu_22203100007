using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    [Table("ViTriDoXe")]
    public class ViTriDoXe
    {
        [Key]
        public int MaViTri { get; set; }

        [Required(ErrorMessage = "Tên vị trí đỗ không được để trống")]
        [StringLength(50)]
        public string TenViTri { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại phương tiện")]
        public int MaLoaiPhuongTien { get; set; }

        [Required(ErrorMessage = "Khu vực không được để trống")]
        [StringLength(20)]
        public string KhuVuc { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Tầng không được để trống")]
        [Range(1, 20, ErrorMessage = "Tầng từ 1 đến 20")]
        public int Tang { get; set; } = 1;

        [Required]
        [StringLength(30)]
        public string TrangThai { get; set; } = "Còn trống"; 

        public string? MoTa { get; set; }

        [ForeignKey("MaLoaiPhuongTien")]
        public virtual LoaiPhuongTien? LoaiPhuongTien { get; set; }
        public virtual ICollection<PhieuGuiXe> DanhSachPhieuGui { get; set; } = new List<PhieuGuiXe>();
    }
}