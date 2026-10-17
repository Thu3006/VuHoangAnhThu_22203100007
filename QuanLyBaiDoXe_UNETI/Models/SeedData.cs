using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            
            if (!context.TaiKhoan.Any())
            {
                var admin = new TaiKhoan
                {
                    TenDangNhap = "admin",
                    MatKhau = PasswordHelper.HashPassword("123456"),
                    HoTen = "Quản Trị Viên",
                    Email = "admin@uneti.edu.vn",
                    VaiTro = "Admin",
                    TrangThai = true
                };

                var user1 = new TaiKhoan
                {
                    TenDangNhap = "khachhang01",
                    MatKhau = PasswordHelper.HashPassword("123456"),
                    HoTen = "Nguyễn Văn A",
                    Email = "nguyenvana@gmail.com",
                    VaiTro = "KhachHang",
                    TrangThai = true
                };

                context.TaiKhoan.AddRange(admin, user1);
                context.SaveChanges();

                var chuXe1 = new ChuPhuongTien
                {
                    MaTaiKhoan = user1.MaTaiKhoan,
                    HoTen = "Nguyễn Văn A",
                    NgaySinh = new DateTime(1995, 5, 20),
                    GioiTinh = "Nam",
                    SoDienThoai = "0987654321",
                    Email = "nguyenvana@gmail.com",
                    DiaChi = "Hà Nội",
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                };
                context.ChuPhuongTien.Add(chuXe1);
                context.SaveChanges();

                var loaiXeMay = new LoaiPhuongTien
                {
                    TenLoaiPhuongTien = "Xe máy",
                    DonGiaTheoGio = 5000,
                    MoTa = "Phương tiện hai bánh gắn máy",
                    TrangThai = true
                };

                var loaiOTo4Cho = new LoaiPhuongTien
                {
                    TenLoaiPhuongTien = "Ô tô 4-7 chỗ",
                    DonGiaTheoGio = 25000,
                    MoTa = "Xe ô tô con gia đình",
                    TrangThai = true
                };

                var loaiXeDapDien = new LoaiPhuongTien
                {
                    TenLoaiPhuongTien = "Xe đạp điện",
                    DonGiaTheoGio = 3000,
                    MoTa = "Phương tiện xe đạp gắn động cơ điện",
                    TrangThai = true
                };

                context.LoaiPhuongTien.AddRange(loaiXeMay, loaiOTo4Cho, loaiXeDapDien);
                context.SaveChanges();

                var danhSachViTri = new List<ViTriDoXe>();
                for (int i = 1; i <= 10; i++)
                {
                    danhSachViTri.Add(new ViTriDoXe
                    {
                        TenViTri = $"A1-{(i < 10 ? "0" + i : i.ToString())}",
                        MaLoaiPhuongTien = loaiXeMay.MaLoaiPhuongTien,
                        KhuVuc = "Khu A",
                        Tang = 1,
                        TrangThai = "Còn trống",
                        MoTa = "Vị trí đỗ xe máy tiêu chuẩn"
                    });
                }
                for (int i = 1; i <= 6; i++)
                {
                    danhSachViTri.Add(new ViTriDoXe
                    {
                        TenViTri = $"B1-{(i < 10 ? "0" + i : i.ToString())}",
                        MaLoaiPhuongTien = loaiOTo4Cho.MaLoaiPhuongTien,
                        KhuVuc = "Khu B",
                        Tang = 1,
                        TrangThai = "Còn trống",
                        MoTa = "Vị trí đỗ ô tô có mái che"
                    });
                }
                for (int i = 1; i <= 4; i++)
                {
                    danhSachViTri.Add(new ViTriDoXe
                    {
                        TenViTri = $"C2-{(i < 10 ? "0" + i : i.ToString())}",
                        MaLoaiPhuongTien = loaiXeDapDien.MaLoaiPhuongTien,
                        KhuVuc = "Khu C",
                        Tang = 2,
                        TrangThai = "Còn trống",
                        MoTa = "Vị trí gần trụ sạc"
                    });
                }
                context.ViTriDoXe.AddRange(danhSachViTri);
                context.SaveChanges();

                var xe1 = new PhuongTien
                {
                    MaChuPhuongTien = chuXe1.MaChuPhuongTien,
                    MaLoaiPhuongTien = loaiXeMay.MaLoaiPhuongTien,
                    BienSoXe = "29A1-123.45",
                    NhanHieu = "Honda Wave Alpha",
                    MauSac = "Đỏ đen",
                    MoTa = "Xe cá nhân",
                    TrangThai = true
                };
                context.PhuongTien.Add(xe1);
                context.SaveChanges();
            }

            if (context.PhieuGuiXe.Count() < 10)
            {
                if (!context.TaiKhoan.Any(t => t.TenDangNhap == "tranvanb"))
                {
                    var userB = new TaiKhoan { TenDangNhap = "tranvanb", MatKhau = PasswordHelper.HashPassword("123456"), HoTen = "Trần Văn Bình", Email = "binhtv@gmail.com", VaiTro = "KhachHang", TrangThai = true };
                    var userC = new TaiKhoan { TenDangNhap = "lethic", MatKhau = PasswordHelper.HashPassword("123456"), HoTen = "Lê Thị Cúc", Email = "cucle@gmail.com", VaiTro = "KhachHang", TrangThai = true };
                    var userD = new TaiKhoan { TenDangNhap = "hoangd", MatKhau = PasswordHelper.HashPassword("123456"), HoTen = "Hoàng Văn Dũng", Email = "dunghv@gmail.com", VaiTro = "KhachHang", TrangThai = true };
                    context.TaiKhoan.AddRange(userB, userC, userD);
                    context.SaveChanges();

                    var chuB = new ChuPhuongTien { MaTaiKhoan = userB.MaTaiKhoan, HoTen = "Trần Văn Bình", SoDienThoai = "0912345678", Email = "binhtv@gmail.com", DiaChi = "Cầu Giấy, Hà Nội", NgayDangKy = DateTime.Now.AddMonths(-3), TrangThai = true };
                    var chuC = new ChuPhuongTien { MaTaiKhoan = userC.MaTaiKhoan, HoTen = "Lê Thị Cúc", SoDienThoai = "0978112233", Email = "cucle@gmail.com", DiaChi = "Hai Bà Trưng, Hà Nội", NgayDangKy = DateTime.Now.AddMonths(-2), TrangThai = true };
                    var chuD = new ChuPhuongTien { MaTaiKhoan = userD.MaTaiKhoan, HoTen = "Hoàng Văn Dũng", SoDienThoai = "0944556677", Email = "dunghv@gmail.com", DiaChi = "Thanh Xuân, Hà Nội", NgayDangKy = DateTime.Now.AddMonths(-1), TrangThai = true };
                    context.ChuPhuongTien.AddRange(chuB, chuC, chuD);
                    context.SaveChanges();
                }

                var listLoai = context.LoaiPhuongTien.ToList();
                var loaiXeMay = listLoai.FirstOrDefault(l => l.TenLoaiPhuongTien == "Xe máy") ?? listLoai.First();
                var loaiOTo = listLoai.FirstOrDefault(l => l.TenLoaiPhuongTien.Contains("Ô tô")) ?? listLoai.First();
                var loaiDapDien = listLoai.FirstOrDefault(l => l.TenLoaiPhuongTien.Contains("đạp điện")) ?? listLoai.First();

                var listChuXe = context.ChuPhuongTien.ToList();
                var chu1 = listChuXe[0];
                var chu2 = listChuXe.Count > 1 ? listChuXe[1] : chu1;
                var chu3 = listChuXe.Count > 2 ? listChuXe[2] : chu1;
                var chu4 = listChuXe.Count > 3 ? listChuXe[3] : chu1;

                if (!context.PhuongTien.Any(p => p.BienSoXe == "30E-888.88"))
                {
                    context.PhuongTien.AddRange(
                        new PhuongTien { MaChuPhuongTien = chu2.MaChuPhuongTien, MaLoaiPhuongTien = loaiOTo.MaLoaiPhuongTien, BienSoXe = "30E-888.88", NhanHieu = "Mazda 3", MauSac = "Đỏ", TrangThai = true },
                        new PhuongTien { MaChuPhuongTien = chu2.MaChuPhuongTien, MaLoaiPhuongTien = loaiXeMay.MaLoaiPhuongTien, BienSoXe = "29B1-678.90", NhanHieu = "Honda SH 150i", MauSac = "Trắng", TrangThai = true },
                        new PhuongTien { MaChuPhuongTien = chu3.MaChuPhuongTien, MaLoaiPhuongTien = loaiOTo.MaLoaiPhuongTien, BienSoXe = "30H-567.89", NhanHieu = "Toyota Cross", MauSac = "Bạc", TrangThai = true },
                        new PhuongTien { MaChuPhuongTien = chu3.MaChuPhuongTien, MaLoaiPhuongTien = loaiDapDien.MaLoaiPhuongTien, BienSoXe = "29MD-012.34", NhanHieu = "VinFast Klara", MauSac = "Xanh", TrangThai = true },
                        new PhuongTien { MaChuPhuongTien = chu4.MaChuPhuongTien, MaLoaiPhuongTien = loaiXeMay.MaLoaiPhuongTien, BienSoXe = "29G1-234.56", NhanHieu = "Yamaha Exciter", MauSac = "Xanh GP", TrangThai = true }
                    );
                    context.SaveChanges();
                }

                var listXe = context.PhuongTien.ToList();
                var listViTri = context.ViTriDoXe.ToList();

                var xeMayList = listXe.Where(x => x.MaLoaiPhuongTien == loaiXeMay.MaLoaiPhuongTien).ToList();
                var oToList = listXe.Where(x => x.MaLoaiPhuongTien == loaiOTo.MaLoaiPhuongTien).ToList();
                var dapDienList = listXe.Where(x => x.MaLoaiPhuongTien == loaiDapDien.MaLoaiPhuongTien).ToList();

                var viTriXeMay = listViTri.Where(v => v.MaLoaiPhuongTien == loaiXeMay.MaLoaiPhuongTien).ToList();
                var viTriOTo = listViTri.Where(v => v.MaLoaiPhuongTien == loaiOTo.MaLoaiPhuongTien).ToList();
                var viTriDapDien = listViTri.Where(v => v.MaLoaiPhuongTien == loaiDapDien.MaLoaiPhuongTien).ToList();

                var danhSachPhieuMoi = new List<PhieuGuiXe>();

                danhSachPhieuMoi.Add(new PhieuGuiXe
                {
                    MaPhuongTien = xeMayList[0].MaPhuongTien,
                    MaViTri = viTriXeMay[0].MaViTri,
                    NgayDangKy = new DateTime(2026, 8, 5, 7, 30, 0),
                    ThoiGianDuKienVao = new DateTime(2026, 8, 5, 8, 0, 0),
                    ThoiGianDuKienRa = new DateTime(2026, 8, 5, 12, 0, 0),
                    ThoiGianVaoThucTe = new DateTime(2026, 8, 5, 8, 5, 0),
                    ThoiGianRaThucTe = new DateTime(2026, 8, 5, 12, 10, 0),
                    DonGiaTheoGio = 5000,
                    ThanhTien = 25000,
                    TrangThai = "Đã lấy xe"
                });
                danhSachPhieuMoi.Add(new PhieuGuiXe
                {
                    MaPhuongTien = (oToList.Count > 0 ? oToList[0] : xeMayList[0]).MaPhuongTien,
                    MaViTri = (viTriOTo.Count > 0 ? viTriOTo[0] : viTriXeMay[0]).MaViTri,
                    NgayDangKy = new DateTime(2026, 8, 12, 8, 0, 0),
                    ThoiGianDuKienVao = new DateTime(2026, 8, 12, 9, 0, 0),
                    ThoiGianDuKienRa = new DateTime(2026, 8, 12, 18, 0, 0),
                    ThoiGianVaoThucTe = new DateTime(2026, 8, 12, 8, 50, 0),
                    ThoiGianRaThucTe = new DateTime(2026, 8, 12, 18, 15, 0),
                    DonGiaTheoGio = 25000,
                    ThanhTien = 250000,
                    TrangThai = "Đã lấy xe"
                });
                danhSachPhieuMoi.Add(new PhieuGuiXe
                {
                    MaPhuongTien = (dapDienList.Count > 0 ? dapDienList[0] : xeMayList[0]).MaPhuongTien,
                    MaViTri = (viTriDapDien.Count > 0 ? viTriDapDien[0] : viTriXeMay[0]).MaViTri,
                    NgayDangKy = new DateTime(2026, 8, 20, 13, 0, 0),
                    ThoiGianDuKienVao = new DateTime(2026, 8, 20, 13, 30, 0),
                    ThoiGianDuKienRa = new DateTime(2026, 8, 20, 17, 30, 0),
                    ThoiGianVaoThucTe = new DateTime(2026, 8, 20, 13, 30, 0),
                    ThoiGianRaThucTe = new DateTime(2026, 8, 20, 17, 30, 0),
                    DonGiaTheoGio = 3000,
                    ThanhTien = 12000,
                    TrangThai = "Đã lấy xe"
                });


                danhSachPhieuMoi.Add(new PhieuGuiXe
                {
                    MaPhuongTien = xeMayList[0].MaPhuongTien,
                    MaViTri = viTriXeMay[1].MaViTri,
                    NgayDangKy = new DateTime(2026, 9, 2, 7, 0, 0),
                    ThoiGianDuKienVao = new DateTime(2026, 9, 2, 7, 30, 0),
                    ThoiGianDuKienRa = new DateTime(2026, 9, 2, 11, 30, 0),
                    ThoiGianVaoThucTe = new DateTime(2026, 9, 2, 7, 30, 0),
                    ThoiGianRaThucTe = new DateTime(2026, 9, 2, 11, 0, 0),
                    DonGiaTheoGio = 5000,
                    ThanhTien = 20000,
                    TrangThai = "Đã lấy xe"
                });
                danhSachPhieuMoi.Add(new PhieuGuiXe
                {
                    MaPhuongTien = (oToList.Count > 1 ? oToList[1] : xeMayList[0]).MaPhuongTien,
                    MaViTri = (viTriOTo.Count > 1 ? viTriOTo[1] : viTriXeMay[0]).MaViTri,
                    NgayDangKy = new DateTime(2026, 9, 10, 8, 0, 0),
                    ThoiGianDuKienVao = new DateTime(2026, 9, 10, 9, 0, 0),
                    ThoiGianDuKienRa = new DateTime(2026, 9, 10, 14, 0, 0),
                    ThoiGianVaoThucTe = new DateTime(2026, 9, 10, 9, 0, 0),
                    ThoiGianRaThucTe = new DateTime(2026, 9, 10, 14, 30, 0),
                    DonGiaTheoGio = 25000,
                    ThanhTien = 150000,
                    TrangThai = "Đã lấy xe"
                });
                danhSachPhieuMoi.Add(new PhieuGuiXe
                {
                    MaPhuongTien = xeMayList[1].MaPhuongTien,
                    MaViTri = viTriXeMay[0].MaViTri,
                    NgayDangKy = new DateTime(2026, 9, 15, 14, 0, 0),
                    ThoiGianDuKienVao = new DateTime(2026, 9, 15, 15, 0, 0),
                    ThoiGianDuKienRa = new DateTime(2026, 9, 15, 18, 0, 0),
                    DonGiaTheoGio = 5000,
                    TrangThai = "Đã hủy"
                });


                for (int d = 1; d <= 12; d++)
                {
                    bool isOTo = (d % 3 == 0) && viTriOTo.Any();
                    var pt = isOTo ? (oToList.Count > 0 ? oToList[d % oToList.Count] : xeMayList[0]) : xeMayList[d % xeMayList.Count];
                    var vt = isOTo ? viTriOTo[d % viTriOTo.Count] : viTriXeMay[d % viTriXeMay.Count];
                    decimal donGia = isOTo ? 25000 : 5000;
                    int soGio = (d % 4) + 2;

                    danhSachPhieuMoi.Add(new PhieuGuiXe
                    {
                        MaPhuongTien = pt.MaPhuongTien,
                        MaViTri = vt.MaViTri,
                        NgayDangKy = new DateTime(2026, 10, d, 7, 15, 0),
                        ThoiGianDuKienVao = new DateTime(2026, 10, d, 7, 30, 0),
                        ThoiGianDuKienRa = new DateTime(2026, 10, d, 7, 30, 0).AddHours(soGio),
                        ThoiGianVaoThucTe = new DateTime(2026, 10, d, 7, 30, 0),
                        ThoiGianRaThucTe = new DateTime(2026, 10, d, 7, 30, 0).AddHours(soGio),
                        DonGiaTheoGio = donGia,
                        ThanhTien = soGio * donGia,
                        TrangThai = "Đã lấy xe"
                    });
                }

                if (viTriXeMay.Count > 2)
                {
                    danhSachPhieuMoi.Add(new PhieuGuiXe
                    {
                        MaPhuongTien = xeMayList[0].MaPhuongTien,
                        MaViTri = viTriXeMay[2].MaViTri,
                        NgayDangKy = DateTime.Now.AddHours(-3),
                        ThoiGianDuKienVao = DateTime.Now.AddHours(-3),
                        ThoiGianDuKienRa = DateTime.Now.AddHours(2),
                        ThoiGianVaoThucTe = DateTime.Now.AddHours(-3),
                        DonGiaTheoGio = 5000,
                        TrangThai = "Đang gửi"
                    });
                    viTriXeMay[2].TrangThai = "Đang sử dụng";
                }
                if (viTriOTo.Count > 2 && oToList.Count > 0)
                {
                    danhSachPhieuMoi.Add(new PhieuGuiXe
                    {
                        MaPhuongTien = oToList[0].MaPhuongTien,
                        MaViTri = viTriOTo[2].MaViTri,
                        NgayDangKy = DateTime.Now.AddHours(-2),
                        ThoiGianDuKienVao = DateTime.Now.AddHours(-2),
                        ThoiGianDuKienRa = DateTime.Now.AddHours(3),
                        ThoiGianVaoThucTe = DateTime.Now.AddHours(-2),
                        DonGiaTheoGio = 25000,
                        TrangThai = "Đang gửi"
                    });
                    viTriOTo[2].TrangThai = "Đang sử dụng";
                }


                if (viTriXeMay.Count > 3)
                {
                    danhSachPhieuMoi.Add(new PhieuGuiXe
                    {
                        MaPhuongTien = xeMayList[1].MaPhuongTien,
                        MaViTri = viTriXeMay[3].MaViTri,
                        NgayDangKy = DateTime.Now.AddMinutes(-30),
                        ThoiGianDuKienVao = DateTime.Now.AddHours(1),
                        ThoiGianDuKienRa = DateTime.Now.AddHours(4),
                        DonGiaTheoGio = 5000,
                        TrangThai = "Chờ xác nhận"
                    });
                }
                if (dapDienList.Count > 0 && viTriDapDien.Count > 0)
                {
                    danhSachPhieuMoi.Add(new PhieuGuiXe
                    {
                        MaPhuongTien = dapDienList[0].MaPhuongTien,
                        MaViTri = viTriDapDien[0].MaViTri,
                        NgayDangKy = DateTime.Now.AddMinutes(-10),
                        ThoiGianDuKienVao = DateTime.Now.AddHours(2),
                        ThoiGianDuKienRa = DateTime.Now.AddHours(5),
                        DonGiaTheoGio = 3000,
                        TrangThai = "Chờ xác nhận"
                    });
                }

                context.PhieuGuiXe.AddRange(danhSachPhieuMoi);
                context.SaveChanges();
            }
        }
    }
}