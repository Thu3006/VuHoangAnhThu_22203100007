using System.Collections.Generic;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Nội dung: ViewModel đóng gói số liệu Dashboard và Thống kê LINQ
    public class DashboardViewModel
    {
        public int TongSoLoaiXe { get; set; }
        public int TongSoViTri { get; set; }
        public int SoViTriTrong { get; set; }
        public int SoViTriDangDung { get; set; }
        public int TongSoChuXe { get; set; }
        public int TongSoXe { get; set; }
        public int TongSoPhieu { get; set; }
        public int SoPhieuChoXacNhan { get; set; }
        public int SoXeDangGui { get; set; }
        public decimal TongDoanhThu { get; set; }

        public List<ThongKeLoaiXeItem> ThongKeTheoLoaiXe { get; set; } = new List<ThongKeLoaiXeItem>();
        public List<ThongKeViTriItem> ThongKeTheoViTri { get; set; } = new List<ThongKeViTriItem>();
        public List<DoanhThuThangItem> DoanhThuTheoThang { get; set; } = new List<DoanhThuThangItem>();
    }

    public class ThongKeLoaiXeItem
    {
        public string TenLoaiXe { get; set; } = string.Empty;
        public int SoLuongXe { get; set; }
        public int SoLuotGui { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class ThongKeViTriItem
    {
        public string TenViTri { get; set; } = string.Empty;
        public string KhuVuc { get; set; } = string.Empty;
        public int Tang { get; set; }
        public int SoLuotSuDung { get; set; }
    }

    public class DoanhThuThangItem
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public int SoLuotGui { get; set; }
        public decimal TongTien { get; set; }
    }
}