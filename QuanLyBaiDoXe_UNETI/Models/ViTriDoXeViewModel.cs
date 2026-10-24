using System.Collections.Generic;
using QuanLyBaiDoXe_UNETI.Models;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    public class ViTriDoXeIndexViewModel
    {
        public List<ViTriDoXe> DanhSachViTri { get; set; } = new List<ViTriDoXe>();

        public string? SearchTerm { get; set; }
        public int? MaLoaiPhuongTien { get; set; }
        public string? KhuVuc { get; set; }
        public int? Tang { get; set; }
        public string? TrangThai { get; set; }
        public string? SortOrder { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 8; 
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}