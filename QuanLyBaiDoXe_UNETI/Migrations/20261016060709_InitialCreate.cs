using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBaiDoXe_UNETI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiPhuongTien",
                columns: table => new
                {
                    MaLoaiPhuongTien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiPhuongTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DonGiaTheoGio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhuongTien", x => x.MaLoaiPhuongTien);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "ViTriDoXe",
                columns: table => new
                {
                    MaViTri = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenViTri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiPhuongTien = table.Column<int>(type: "int", nullable: false),
                    KhuVuc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViTriDoXe", x => x.MaViTri);
                    table.ForeignKey(
                        name: "FK_ViTriDoXe_LoaiPhuongTien_MaLoaiPhuongTien",
                        column: x => x.MaLoaiPhuongTien,
                        principalTable: "LoaiPhuongTien",
                        principalColumn: "MaLoaiPhuongTien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChuPhuongTien",
                columns: table => new
                {
                    MaChuPhuongTien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuPhuongTien", x => x.MaChuPhuongTien);
                    table.ForeignKey(
                        name: "FK_ChuPhuongTien_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTien",
                columns: table => new
                {
                    MaPhuongTien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChuPhuongTien = table.Column<int>(type: "int", nullable: false),
                    MaLoaiPhuongTien = table.Column<int>(type: "int", nullable: false),
                    BienSoXe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NhanHieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MauSac = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTien", x => x.MaPhuongTien);
                    table.ForeignKey(
                        name: "FK_PhuongTien_ChuPhuongTien_MaChuPhuongTien",
                        column: x => x.MaChuPhuongTien,
                        principalTable: "ChuPhuongTien",
                        principalColumn: "MaChuPhuongTien",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhuongTien_LoaiPhuongTien_MaLoaiPhuongTien",
                        column: x => x.MaLoaiPhuongTien,
                        principalTable: "LoaiPhuongTien",
                        principalColumn: "MaLoaiPhuongTien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuGuiXe",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhuongTien = table.Column<int>(type: "int", nullable: false),
                    MaViTri = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianDuKienVao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianDuKienRa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianVaoThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianRaThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DonGiaTheoGio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuGuiXe", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK_PhieuGuiXe_PhuongTien_MaPhuongTien",
                        column: x => x.MaPhuongTien,
                        principalTable: "PhuongTien",
                        principalColumn: "MaPhuongTien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuGuiXe_ViTriDoXe_MaViTri",
                        column: x => x.MaViTri,
                        principalTable: "ViTriDoXe",
                        principalColumn: "MaViTri",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChuPhuongTien_MaTaiKhoan",
                table: "ChuPhuongTien",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoaiPhuongTien_TenLoaiPhuongTien",
                table: "LoaiPhuongTien",
                column: "TenLoaiPhuongTien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGuiXe_MaPhuongTien",
                table: "PhieuGuiXe",
                column: "MaPhuongTien");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGuiXe_MaViTri",
                table: "PhieuGuiXe",
                column: "MaViTri");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTien_BienSoXe",
                table: "PhuongTien",
                column: "BienSoXe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTien_MaChuPhuongTien",
                table: "PhuongTien",
                column: "MaChuPhuongTien");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTien_MaLoaiPhuongTien",
                table: "PhuongTien",
                column: "MaLoaiPhuongTien");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViTriDoXe_MaLoaiPhuongTien",
                table: "ViTriDoXe",
                column: "MaLoaiPhuongTien");

            migrationBuilder.CreateIndex(
                name: "IX_ViTriDoXe_TenViTri",
                table: "ViTriDoXe",
                column: "TenViTri",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhieuGuiXe");

            migrationBuilder.DropTable(
                name: "PhuongTien");

            migrationBuilder.DropTable(
                name: "ViTriDoXe");

            migrationBuilder.DropTable(
                name: "ChuPhuongTien");

            migrationBuilder.DropTable(
                name: "LoaiPhuongTien");

            migrationBuilder.DropTable(
                name: "TaiKhoan");
        }
    }
}
