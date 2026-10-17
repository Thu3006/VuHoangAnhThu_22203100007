using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    public class DangKyGuiXeViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn phương tiện gửi")]
        [Display(Name = "Phương tiện gửi")]
        public int MaPhuongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vị trí đỗ")]
        [Display(Name = "Vị trí đỗ")]
        public int MaViTri { get; set; }

        [Required(ErrorMessage = "Thời gian vào dự kiến bắt buộc")]
        [Display(Name = "Thời gian vào dự kiến")]
        public DateTime ThoiGianDuKienVao { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Thời gian ra dự kiến bắt buộc")]
        [Display(Name = "Thời gian ra dự kiến")]
        public DateTime ThoiGianDuKienRa { get; set; } = DateTime.Now.AddHours(2);
    }
}