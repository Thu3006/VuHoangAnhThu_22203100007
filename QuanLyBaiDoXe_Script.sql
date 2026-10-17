IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [LoaiPhuongTien] (
    [MaLoaiPhuongTien] int NOT NULL IDENTITY,
    [TenLoaiPhuongTien] nvarchar(50) NOT NULL,
    [DonGiaTheoGio] decimal(18,2) NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_LoaiPhuongTien] PRIMARY KEY ([MaLoaiPhuongTien])
);
GO

CREATE TABLE [TaiKhoan] (
    [MaTaiKhoan] int NOT NULL IDENTITY,
    [TenDangNhap] nvarchar(50) NOT NULL,
    [MatKhau] nvarchar(255) NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [VaiTro] nvarchar(20) NOT NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_TaiKhoan] PRIMARY KEY ([MaTaiKhoan])
);
GO

CREATE TABLE [ViTriDoXe] (
    [MaViTri] int NOT NULL IDENTITY,
    [TenViTri] nvarchar(50) NOT NULL,
    [MaLoaiPhuongTien] int NOT NULL,
    [KhuVuc] nvarchar(20) NOT NULL,
    [Tang] int NOT NULL,
    [TrangThai] nvarchar(30) NOT NULL,
    [MoTa] nvarchar(max) NULL,
    CONSTRAINT [PK_ViTriDoXe] PRIMARY KEY ([MaViTri]),
    CONSTRAINT [FK_ViTriDoXe_LoaiPhuongTien_MaLoaiPhuongTien] FOREIGN KEY ([MaLoaiPhuongTien]) REFERENCES [LoaiPhuongTien] ([MaLoaiPhuongTien]) ON DELETE CASCADE
);
GO

CREATE TABLE [ChuPhuongTien] (
    [MaChuPhuongTien] int NOT NULL IDENTITY,
    [MaTaiKhoan] int NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [NgaySinh] datetime2 NULL,
    [GioiTinh] nvarchar(10) NULL,
    [SoDienThoai] nvarchar(15) NOT NULL,
    [Email] nvarchar(100) NULL,
    [DiaChi] nvarchar(255) NULL,
    [NgayDangKy] datetime2 NOT NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_ChuPhuongTien] PRIMARY KEY ([MaChuPhuongTien]),
    CONSTRAINT [FK_ChuPhuongTien_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
);
GO

CREATE TABLE [PhuongTien] (
    [MaPhuongTien] int NOT NULL IDENTITY,
    [MaChuPhuongTien] int NOT NULL,
    [MaLoaiPhuongTien] int NOT NULL,
    [BienSoXe] nvarchar(20) NOT NULL,
    [NhanHieu] nvarchar(50) NULL,
    [MauSac] nvarchar(30) NULL,
    [MoTa] nvarchar(max) NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_PhuongTien] PRIMARY KEY ([MaPhuongTien]),
    CONSTRAINT [FK_PhuongTien_ChuPhuongTien_MaChuPhuongTien] FOREIGN KEY ([MaChuPhuongTien]) REFERENCES [ChuPhuongTien] ([MaChuPhuongTien]) ON DELETE CASCADE,
    CONSTRAINT [FK_PhuongTien_LoaiPhuongTien_MaLoaiPhuongTien] FOREIGN KEY ([MaLoaiPhuongTien]) REFERENCES [LoaiPhuongTien] ([MaLoaiPhuongTien]) ON DELETE CASCADE
);
GO

CREATE TABLE [PhieuGuiXe] (
    [MaPhieu] int NOT NULL IDENTITY,
    [MaPhuongTien] int NOT NULL,
    [MaViTri] int NOT NULL,
    [NgayDangKy] datetime2 NOT NULL,
    [ThoiGianDuKienVao] datetime2 NOT NULL,
    [ThoiGianDuKienRa] datetime2 NOT NULL,
    [ThoiGianVaoThucTe] datetime2 NULL,
    [ThoiGianRaThucTe] datetime2 NULL,
    [DonGiaTheoGio] decimal(18,2) NOT NULL,
    [ThanhTien] decimal(18,2) NULL,
    [TrangThai] nvarchar(30) NOT NULL,
    CONSTRAINT [PK_PhieuGuiXe] PRIMARY KEY ([MaPhieu]),
    CONSTRAINT [FK_PhieuGuiXe_PhuongTien_MaPhuongTien] FOREIGN KEY ([MaPhuongTien]) REFERENCES [PhuongTien] ([MaPhuongTien]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PhieuGuiXe_ViTriDoXe_MaViTri] FOREIGN KEY ([MaViTri]) REFERENCES [ViTriDoXe] ([MaViTri]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_ChuPhuongTien_MaTaiKhoan] ON [ChuPhuongTien] ([MaTaiKhoan]);
GO

CREATE UNIQUE INDEX [IX_LoaiPhuongTien_TenLoaiPhuongTien] ON [LoaiPhuongTien] ([TenLoaiPhuongTien]);
GO

CREATE INDEX [IX_PhieuGuiXe_MaPhuongTien] ON [PhieuGuiXe] ([MaPhuongTien]);
GO

CREATE INDEX [IX_PhieuGuiXe_MaViTri] ON [PhieuGuiXe] ([MaViTri]);
GO

CREATE UNIQUE INDEX [IX_PhuongTien_BienSoXe] ON [PhuongTien] ([BienSoXe]);
GO

CREATE INDEX [IX_PhuongTien_MaChuPhuongTien] ON [PhuongTien] ([MaChuPhuongTien]);
GO

CREATE INDEX [IX_PhuongTien_MaLoaiPhuongTien] ON [PhuongTien] ([MaLoaiPhuongTien]);
GO

CREATE UNIQUE INDEX [IX_TaiKhoan_TenDangNhap] ON [TaiKhoan] ([TenDangNhap]);
GO

CREATE INDEX [IX_ViTriDoXe_MaLoaiPhuongTien] ON [ViTriDoXe] ([MaLoaiPhuongTien]);
GO

CREATE UNIQUE INDEX [IX_ViTriDoXe_TenViTri] ON [ViTriDoXe] ([TenViTri]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261016060709_InitialCreate', N'8.0.32');
GO

COMMIT;
GO

