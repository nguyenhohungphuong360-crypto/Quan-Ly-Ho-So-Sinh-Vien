use QLHSSV;
go

-- VIEW
-- Tạo View hiển thị danh sách sinh viên
CREATE VIEW dbo.vw_SinhVienDayDu
AS
SELECT
    sv.MaSV,
    sv.HoTen,
    sv.CCCD,
    sv.NgaySinh,
    sv.GioiTinh,
    sv.SDT,
    sv.Email,
    sv.DiaChi,
    sv.DanToc,
    sv.TrangThaiHocTap,
    sv.MaLop,
    l.TenLop,
    l.NienKhoa,
    n.MaNganh,
    n.TenNganh,
    k.MaKhoa,
    k.TenKhoa
FROM dbo.SinhVien sv
JOIN dbo.Lop l
    ON sv.MaLop = l.MaLop
JOIN dbo.Nganh n
    ON l.MaNganh = n.MaNganh
JOIN dbo.Khoa k
    ON n.MaKhoa = k.MaKhoa;
GO

-- View lấy đầy đủ thông tin lớp
CREATE OR ALTER VIEW dbo.vw_LopDayDu
AS
SELECT
    l.MaLop,
    l.TenLop,
    l.NienKhoa,
    l.MaNganh,
    n.TenNganh
FROM dbo.Lop l
JOIN dbo.Nganh n
    ON l.MaNganh = n.MaNganh;
GO

-- View lấy đầy đủ thông tin người thân
CREATE VIEW dbo.vw_NguoiThanDayDu
AS
SELECT
    nt.MaNT,
    nt.HoTen,
    nt.MoiQuanHe,
    nt.NgheNghiep,
    nt.SDT,
    nt.NgaySinh,
    nt.DiaChi,
    nt.DanToc,
    nt.GioiTinh,
    nt.MaSV
FROM dbo.NguoiThan nt;
GO

-- View đầy đủ thông tin ngành
CREATE VIEW dbo.vw_NganhDayDu
AS
SELECT
    n.MaNganh,
    n.TenNganh,
    n.MaKhoa,
    k.TenKhoa
FROM dbo.Nganh n
JOIN dbo.Khoa k
    ON n.MaKhoa = k.MaKhoa;
GO

-- View thông tin khoa
CREATE VIEW dbo.vw_KhoaDayDu
AS
SELECT
    MaKhoa,
    TenKhoa,
    SDT,
    Email,
    DiaChi
FROM dbo.Khoa;
GO

-- View đầy đủ CTĐT
CREATE VIEW dbo.vw_CTDTDayDu
AS
SELECT
    ct.MaCTDT,
    ct.TenCTDT,
    ct.TrinhDo,
    ct.ThoiGianDaoTao,
    ct.TongTinChi,
    ct.NamBatDau,
    ct.NamKetThuc,
    ct.MaNganh,
    n.TenNganh
FROM dbo.ChuongTrinhDaoTao ct
JOIN dbo.Nganh n
    ON ct.MaNganh = n.MaNganh;
GO

-- View tài khoản
CREATE VIEW dbo.vw_TaiKhoan
AS
SELECT
    TenDangNhap,
    MatKhau,
    VaiTro,
    TrangThai
FROM dbo.TaiKhoan;
GO

-- View hiển thị lịch sử xóa
CREATE view dbo.vw_LichSuXoaSinhVien
AS
SELECT
    Id,
    MaSV,
    HoTen,
    CCCD,
    MaLop,
    ThoiGianXoa
FROM dbo.LichSuXoaSinhVien;
GO

-- Tạo View hiển thị số lượng sinh viên theo lớp
create view vw_SLSV_Lop
AS
SELECT
    l.MaLop,
    l.TenLop,
    COUNT(s.MaSV) AS SoLuongSinhVien
FROM dbo.Lop l
LEFT JOIN dbo.SinhVien s
    ON l.MaLop = s.MaLop
GROUP BY
    l.MaLop,
    l.TenLop;
GO

-- Thống kê số sinh viên theo giới tính
CREATE OR ALTER VIEW dbo.vw_SLSV_GioiTinh
AS
SELECT
    GioiTinh,
    COUNT(*) AS SoLuongSinhVien
FROM dbo.SinhVien
GROUP BY GioiTinh;
GO
-- ===================================
-- PROCEDURE 
-- ===================================
-- Đăng nhập tài khoản 
CREATE PROCEDURE dbo.sp_DangNhap
    @TenDangNhap VARCHAR(50),
    @MatKhau VARCHAR(100)
AS
BEGIN
    SELECT TenDangNhap, MatKhau, VaiTro, TrangThai
    FROM dbo.TaiKhoan
    WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau AND TrangThai = 1
END
GO
-- Thêm tài khoản

CREATE OR ALTER PROCEDURE dbo.sp_ThemTaiKhoan
    @TenDangNhap NVARCHAR(50),
    @MatKhau VARCHAR(100),
    @VaiTro NVARCHAR(20),
    @TrangThai BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.TaiKhoan
        (TenDangNhap, MatKhau, VaiTro, TrangThai)
    VALUES
        (@TenDangNhap, @MatKhau, @VaiTro, @TrangThai);
END;
GO

-- Sửa tài khoản
CREATE OR ALTER PROCEDURE dbo.sp_SuaTaiKhoan
    @TenDangNhap NVARCHAR(50),
    @MatKhau VARCHAR(100),
    @VaiTro NVARCHAR(20),
    @TrangThai BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.TaiKhoan
    SET
        MatKhau = @MatKhau,
        VaiTro = @VaiTro,
        TrangThai = @TrangThai
    WHERE TenDangNhap = @TenDangNhap;
END;
GO
-- xóa tài khoản
CREATE OR ALTER PROCEDURE dbo.sp_XoaTaiKhoan
    @TenDangNhap NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Chặn không cho xóa tài khoản admin gốc
    IF @TenDangNhap = N'admin'
    BEGIN
        SELECT 0;
        RETURN;
    END
    -- Tiến hành xóa tài khoản nhân viên bình thường
    DELETE FROM dbo.TaiKhoan WHERE TenDangNhap = @TenDangNhap;
    SELECT 1;
END;
GO

-- Tìm tài khoản
CREATE OR ALTER PROCEDURE dbo.sp_TimTaiKhoan
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        TenDangNhap,
        MatKhau,
        VaiTro,
        TrangThai
    FROM dbo.vw_TaiKhoan
    WHERE
        TenDangNhap LIKE N'%' + @TuKhoa + N'%'
        OR VaiTro LIKE N'%' + @TuKhoa + N'%'
    ORDER BY TenDangNhap;
END;
GO
-- Tạo Procedure kiểm tra tài khoản tồn tại
CREATE OR ALTER PROCEDURE dbo.sp_KiemTraTaiKhoanTonTai
    @TenDangNhap NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.TaiKhoan
        WHERE TenDangNhap = @TenDangNhap
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END;
GO
-- Tạo Procedure thêm Sinh Viên
create procedure sp_ThemSinhVien
    @MaSV varchar(10),
    @HoTen nvarchar(50),
    @CCCD varchar(20),
    @NgaySinh date,
    @GioiTinh nvarchar(10),
    @SDT varchar(20),
    @Email varchar(50),
    @DiaChi nvarchar(100),
    @DanToc nvarchar(20),
    @TrangThaiHocTap nvarchar(50),
    @MaLop varchar(10)
as
begin
    insert into SinhVien
    (MaSV,HoTen,CCCD,
    NgaySinh,GioiTinh,SDT,
    Email,DiaChi,DanToc,
    TrangThaiHocTap,MaLop)
    values
    (@MaSV,@HoTen,@CCCD,@NgaySinh,@GioiTinh,@SDT,
    @Email,@DiaChi,@DanToc,@TrangThaiHocTap,@MaLop);
end;
go
--  Tạo Procedure sửa Sinh Viên 
create procedure sp_SuaSinhVien
    @MaSV varchar(10),
    @HoTen nvarchar(50),
    @CCCD varchar(20),
    @NgaySinh date,
    @GioiTinh nvarchar(10),
    @SDT varchar(20),
    @Email varchar(50),
    @DiaChi nvarchar(100),
    @DanToc nvarchar(20),
    @TrangThaiHocTap nvarchar(50),
    @MaLop varchar(10)
as
begin
    update SinhVien
    set
        HoTen = @HoTen,
        CCCD = @CCCD,
        NgaySinh = @NgaySinh,
        GioiTinh = @GioiTinh,
        SDT = @SDT,
        Email = @Email,
        DiaChi = @DiaChi,
        DanToc = @DanToc,
        TrangThaiHocTap = @TrangThaiHocTap,
        MaLop = @MaLop
    where MaSV = @MaSV;
end;
go
-- Tạo Procedure xóa Sinh Viên 
create procedure sp_XoaSinhVien
    @MaSV VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Không tìm thấy sinh viên
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.SinhVien
        WHERE MaSV = @MaSV
    )
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Xóa người thân trước
        DELETE FROM dbo.NguoiThan
        WHERE MaSV = @MaSV;

        -- Xóa sinh viên
        DELETE FROM dbo.SinhVien
        WHERE MaSV = @MaSV;

        COMMIT TRANSACTION;

        -- Báo C# xóa thành công
        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO
-- PROCEDURE TÌM KIẾM SINH VIÊN

CREATE PROCEDURE dbo.sp_TimSinhVien
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Tìm sinh viên có dữ liệu chứa từ khóa
    SELECT
        MaSV,
        HoTen,
        CCCD,
        NgaySinh,
        GioiTinh,
        SDT,
        Email,
        DiaChi,
        DanToc,
        TrangThaiHocTap,
        MaLop
    FROM dbo.vw_SinhVienDayDu
    WHERE
        MaSV LIKE '%' + @TuKhoa + '%'
        OR HoTen LIKE N'%' + @TuKhoa + N'%'
        OR CCCD LIKE '%' + @TuKhoa + '%'
    ORDER BY MaSV;
END;
GO

-- Kiểm tra tính tồn tại của sinh viên
CREATE procedure dbo.sp_KiemTraSinhVienTonTai
    @MaSV VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.SinhVien
        WHERE MaSV = @MaSV
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END;
GO

-- Tạo Procedure thêm Lớp 
create procedure sp_ThemLop
    @MaLop VARCHAR(10),
    @TenLop NVARCHAR(50),
    @NienKhoa NVARCHAR(10),
    @MaNganh VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Lop
        (MaLop, TenLop, NienKhoa, MaNganh)
    VALUES
        (@MaLop, @TenLop, @NienKhoa, @MaNganh);
END;
GO

-- Sửa lớp
CREATE PROCEDURE dbo.sp_SuaLop
    @MaLop VARCHAR(10),
    @TenLop NVARCHAR(50),
    @NienKhoa NVARCHAR(10),
    @MaNganh VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Lop
    SET
        TenLop = @TenLop,
        NienKhoa = @NienKhoa,
        MaNganh = @MaNganh
    WHERE MaLop = @MaLop;
END;
GO

-- Tạo Procedure Tìm Lớp 
CREATE OR ALTER PROCEDURE dbo.sp_TimLop
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MaLop,
        TenLop,
        NienKhoa,
        MaNganh
    FROM dbo.vw_LopDayDu
    WHERE
        MaLop LIKE '%' + @TuKhoa + '%'
        OR TenLop LIKE N'%' + @TuKhoa + N'%'
        OR NienKhoa LIKE N'%' + @TuKhoa + N'%'
        OR MaNganh LIKE '%' + @TuKhoa + '%'
        OR TenNganh LIKE N'%' + @TuKhoa + N'%'
    ORDER BY MaLop;
END;
GO


-- Tạo Procedure Xóa Lớp 
CREATE PROCEDURE dbo.sp_XoaLop
    @MaLop VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Lop
        WHERE MaLop = @MaLop
    )
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END;

    -- Không xóa lớp đang có sinh viên
    IF EXISTS (
        SELECT 1
        FROM dbo.SinhVien
        WHERE MaLop = @MaLop
    )
    BEGIN
        THROW 50010, N'Không thể xóa lớp đang có sinh viên!', 1;
    END;

    DELETE FROM dbo.Lop
    WHERE MaLop = @MaLop;

    SELECT CAST(1 AS BIT);
END;
GO

-- Kiểm tra mã lớp tồn tại
CREATE OR ALTER PROCEDURE dbo.sp_KiemTraLopTonTai
    @MaLop VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Lop
        WHERE MaLop = @MaLop
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END;
GO
-- Tạo Procedure Chuyển Lớp 
create procedure sp_ChuyenLop
    @MaSV varchar(10),
    @MaLopMoi varchar(10)
as
begin
    update SinhVien
    set MaLop = @MaLopMoi
    where MaSV = @MaSV;
end;
go
-- Tạo Procedure thêm Ngành 
create procedure sp_ThemNganh
    @MaNganh varchar(10),
    @TenNganh nvarchar(50),
    @MaKhoa varchar(10)
as
begin
    insert into Nganh 
    (MaNganh,TenNganh,MaKhoa)
    values
    (@MaNganh,@TenNganh,@MaKhoa);
end;
go

-- Tạo Procedure sửa Ngành 
create procedure sp_SuaNganh
    @MaNganh varchar(10),
    @TenNganh nvarchar(50),
    @MaKhoa varchar(10)
as
begin
    update Nganh
    set
        TenNganh = @TenNganh,
        MaKhoa = @MaKhoa
    where MaNganh = @MaNganh;
end;
go


-- Tạo Procedure xóa Ngành 
CREATE OR ALTER PROCEDURE dbo.sp_XoaNganh
    @MaNganh VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Nganh
        WHERE MaNganh = @MaNganh
    )
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Lop
        WHERE MaNganh = @MaNganh
    )
    BEGIN
        THROW 50020,
              N'Không thể xóa ngành vì đang có lớp thuộc ngành này!',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.ChuongTrinhDaoTao
        WHERE MaNganh = @MaNganh
    )
    BEGIN
        THROW 50021,
              N'Không thể xóa ngành vì đang có chương trình đào tạo thuộc ngành này!',
              1;
    END;

    DELETE FROM dbo.Nganh
    WHERE MaNganh = @MaNganh;

    SELECT CAST(1 AS BIT);
END;
GO

-- Tìm ngành
CREATE OR ALTER PROCEDURE dbo.sp_TimNganh
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        n.MaNganh,
        n.TenNganh,
        n.MaKhoa
    FROM dbo.Nganh AS n
    LEFT JOIN dbo.Khoa AS k
        ON n.MaKhoa = k.MaKhoa
    WHERE
        n.MaNganh LIKE '%' + @TuKhoa + '%'
        OR n.TenNganh LIKE N'%' + @TuKhoa + N'%'
        OR n.MaKhoa LIKE '%' + @TuKhoa + '%'
        OR k.TenKhoa LIKE N'%' + @TuKhoa + N'%'
    ORDER BY n.MaNganh;
END;
GO

-- Kiểm tra mã ngành tồn tại
CREATE PROCEDURE dbo.sp_KiemTraNganhTonTai
    @MaNganh VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Nganh
        WHERE MaNganh = @MaNganh
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END;
GO

-- Tạo Procedure thêm Khoa
create procedure sp_ThemKhoa
    @MaKhoa varchar (10),
	@TenKhoa nvarchar(50),
    @SDT varchar(20),
    @Email varchar(50),
    @DiaChi nvarchar(100)
as
begin
    insert into Khoa
    (MaKhoa,TenKhoa,SDT,Email,DiaChi)
    values
    (@MaKhoa,@TenKhoa,@SDT,@Email,@DiaChi);
end;
go
--Tìm Khoa
CREATE OR ALTER PROCEDURE dbo.sp_TimKhoa
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MaKhoa,
        TenKhoa,
        SDT,
        Email,
        DiaChi
    FROM dbo.vw_KhoaDayDu
    WHERE
        MaKhoa LIKE '%' + @TuKhoa + '%'
        OR TenKhoa LIKE N'%' + @TuKhoa + '%'
        OR SDT LIKE '%' + @TuKhoa + '%'
        OR Email LIKE '%' + @TuKhoa + '%'
        OR DiaChi LIKE N'%' + @TuKhoa + '%'
    ORDER BY MaKhoa;
END;
GO
-- Tạo Procedure sửa Khoa
create procedure sp_SuaKhoa
    @MaKhoa varchar(10),
    @TenKhoa nvarchar(50),
    @SDT varchar(20),
    @Email varchar(50),
    @DiaChi nvarchar(100)
as
begin
    update Khoa
    set
        TenKhoa = @TenKhoa,
        SDT = @SDT,
        Email = @Email,
        DiaChi = @DiaChi
    where MaKhoa = @MaKhoa;
end;
go


-- Tạo Procedure xóa Khoa
CREATE OR ALTER PROCEDURE dbo.sp_XoaKhoa
    @MaKhoa VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Khoa
        WHERE MaKhoa = @MaKhoa
    )
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Nganh
        WHERE MaKhoa = @MaKhoa
    )
    BEGIN
        THROW 50030,
              N'Không thể xóa khoa vì đang có ngành thuộc khoa này!',
              1;
    END;

    DELETE FROM dbo.Khoa
    WHERE MaKhoa = @MaKhoa;

    SELECT CAST(1 AS BIT);
END;
GO


-- Kiểm tra mã khoa tồn tại
CREATE  PROCEDURE dbo.sp_KiemTraKhoaTonTai
    @MaKhoa VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Khoa
        WHERE MaKhoa = @MaKhoa
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END;
GO

-- Tạo Procedure thêm Chương Trình Đào Tạo
create procedure sp_ThemCTDT
    @MaCTDT varchar(10),
    @TenCTDT nvarchar(50),
    @TrinhDo nvarchar(20),
    @ThoiGianDaoTao int,
    @TongTinChi int,
    @NamBatDau int,
    @NamKetThuc int,
    @MaNganh varchar(10)
as 
begin
    insert into ChuongTrinhDaoTao
    (MaCTDT,TenCTDT,TrinhDo,ThoiGianDaoTao,
    TongTinChi,NamBatDau,NamKetThuc,MaNganh)
    values
    (@MaCTDT,@TenCTDT,@TrinhDo,@ThoiGianDaoTao,
    @TongTinChi,@NamBatDau,@NamKetThuc,@MaNganh);
end;
go
--Tìm CTDT
CREATE OR ALTER PROCEDURE dbo.sp_TimCTDT
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MaCTDT,
        TenCTDT,
        TrinhDo,
        ThoiGianDaoTao,
        TongTinChi,
        NamBatDau,
        NamKetThuc,
        MaNganh,
        TenNganh
    FROM dbo.vw_CTDTDayDu
    WHERE
        MaCTDT LIKE '%' + @TuKhoa + '%'
        OR TenCTDT LIKE N'%' + @TuKhoa + '%'
        OR TrinhDo LIKE N'%' + @TuKhoa + '%'
        OR MaNganh LIKE '%' + @TuKhoa + '%'
        OR TenNganh LIKE N'%' + @TuKhoa + '%'
    ORDER BY MaCTDT;
END;
GO
-- Tạo Procedure sửa Chương Trình Đào Tạo
create procedure sp_SuaCTDT
    @MaCTDT varchar(10),
    @TenCTDT nvarchar(50),
    @TrinhDo nvarchar(20),
    @ThoiGianDaoTao int,
    @TongTinChi int,
    @NamBatDau int,
    @NamKetThuc int,
    @MaNganh varchar(10)
as
begin
    update ChuongTrinhDaoTao
    set
        TenCTDT = @TenCTDT,
        TrinhDo = @TrinhDo,
        ThoiGianDaoTao = @ThoiGianDaoTao,
        TongTinChi = @TongTinChi,
        NamBatDau = @NamBatDau,
        NamKetThuc = @NamKetThuc,
        MaNganh = @MaNganh
    where MaCTDT = @MaCTDT;
end;
go
-- Xóa CTDT
CREATE OR ALTER PROCEDURE dbo.sp_XoaCTDT
    @MaCTDT VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- Không tìm thấy CTĐT
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.ChuongTrinhDaoTao
        WHERE MaCTDT = @MaCTDT
    )
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END;

    -- Xóa CTĐT
    DELETE FROM dbo.ChuongTrinhDaoTao
    WHERE MaCTDT = @MaCTDT;

    SELECT CAST(1 AS BIT);
END;
GO

-- Kiểm tra mã CTĐT tồn tại
CREATE PROCEDURE dbo.sp_KiemTraCTDTTonTai
    @MaCTDT VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.ChuongTrinhDaoTao
        WHERE MaCTDT = @MaCTDT
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END;
GO

-- Tạo Procedure thêm Người Thân
create procedure sp_ThemNT
    @MaNT varchar(10),
    @HoTen nvarchar(50),
    @MoiQuanHe nvarchar(20),
    @NgheNghiep nvarchar(20),
    @SDT varchar(20),
    @NgaySinh date,
    @DiaChi nvarchar(100),
    @DanToc nvarchar(20),
    @GioiTinh nvarchar(10),
    @MaSV varchar(10)
as
begin
    insert into NguoiThan 
    (MaNT,HoTen,MoiQuanHe,NgheNghiep,SDT,
    NgaySinh,DiaChi,DanToc,GioiTinh,MaSV)
    values
    (@MaNT,@HoTen,@MoiQuanHe,@NgheNghiep,@SDT,
     @NgaySinh,@DiaChi,@DanToc,@GioiTinh,@MaSV);
end;
go

-- Tạo Procedure sửa Người Thân
create procedure sp_SuaNT
    @MaNT varchar(10),
    @HoTen nvarchar(50),
    @MoiQuanHe nvarchar(20),
    @NgheNghiep nvarchar(20),
    @SDT varchar(20),
    @NgaySinh date,
    @DiaChi nvarchar(100),
    @DanToc nvarchar(20),
    @GioiTinh nvarchar(10),
    @MaSV varchar(10)
as
begin
    update NguoiThan
    set
        HoTen = @HoTen,
        MoiQuanHe = @MoiQuanHe,
        NgheNghiep = @NgheNghiep,
        SDT = @SDT,
        NgaySinh = @NgaySinh,
        DiaChi = @DiaChi,
        DanToc = @DanToc,
        GioiTinh = @GioiTinh,
        MaSV = @MaSV
    where MaNT = @MaNT;
end;
go

-- Tạo Procedure xóa Người Thân
create procedure sp_XoaNT
    @MaNT varchar(10)
as
begin
    delete from NguoiThan
    where MaNT = @MaNT;
end;
go

-- Tìm người thân
CREATE PROCEDURE dbo.sp_TimNguoiThan
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.vw_NguoiThanDayDu
    WHERE
        MaNT LIKE '%' + @TuKhoa + '%'
        OR HoTen LIKE N'%' + @TuKhoa + N'%'
        OR MaSV LIKE '%' + @TuKhoa + '%'
    ORDER BY MaNT;
END;
GO

-- Kiểm tra người thân tồn tại
CREATE OR ALTER PROCEDURE dbo.sp_KiemTraNguoiThanTonTai
    @MaNT VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.NguoiThan
        WHERE MaNT = @MaNT
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END;
GO

-- ===================================
-- FUNCTION
-- ===================================

-- Tạo function đếm số sinh viên trong một lớp 
create function fn_DemSVLop
(
    @MaLop varchar(10)
)
returns int
as
begin
    declare @SoSinhVien int;

    select @SoSinhVien = count(*)
    from SinhVien
    where MaLop = @MaLop;

    return @SoSinhVien;
end;
go

select dbo.fn_DemSVLop('L02') as SoSinhVien;
go

-- Tạo function đếm số sinh viên trong khoa 
create function fn_DemSSVKhoa
(
    @MaKhoa varchar(10)
)
returns int
as
begin
    declare @SoSinhVien int;
    
    select @SoSinhVien = count(*)
    from SinhVien
    join Lop on SinhVien.MaLop = Lop.MaLop
    join Nganh on Lop.MaNganh = Nganh.MaNganh
    where Nganh.MaKhoa = @MaKhoa;

    return @SoSinhVien;
end;
go
-- Tạo function đếm số ngành trong khoa 
create function fn_DemSoNganhKhoa
(
    @MaKhoa varchar(10)
)
returns int
as
begin
    declare @SoNganh int;

    select @SoNganh = count(*)
    from Nganh
    where MaKhoa = @MaKhoa;

    return @SoNganh;
end;
go

-- Tạo function đếm số sinh viên thuộc một chương trình đào tạo 
create function fn_DemSVCTDT
(
    @MaCTDT varchar(10)
)
returns int
as
begin
    declare @SoSinhVien int;

    select @SoSinhVien = count(*)
    from ChuongTrinhDaoTao
    where MaCTDT = @MaCTDT;

    return @SoSinhVien;
end;
go

-- ===================================
-- TRIGGER
-- ===================================
-- Tạo Trigger kiểm tra khi thêm sinh viên mới
create trigger trg_KTThemSV
ON dbo.SinhVien
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- =====================================
    -- 1. Kiểm tra CCCD không được để trống
    -- =====================================
    IF EXISTS
    (
        SELECT 1
        FROM inserted
        WHERE CCCD IS NULL
           OR LTRIM(RTRIM(CCCD)) = ''
    )
    BEGIN
        -- Hủy thao tác thêm sinh viên
        ROLLBACK TRANSACTION;

        -- Trả lỗi về cho ứng dụng C#
        THROW 50001,
              N'CCCD không được để trống!',
              1;
    END;

    -- =====================================
    -- 2. Kiểm tra mã lớp có tồn tại không
    -- =====================================
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.Lop l
            WHERE l.MaLop = i.MaLop
        )
    )
    BEGIN
        -- Hủy thao tác thêm sinh viên
        ROLLBACK TRANSACTION;

        -- Trả lỗi về cho ứng dụng C#
        THROW 50002,
              N'Mã lớp không tồn tại!',
              1;
    END;
END;
GO


-- Tạo trigger kiểm tra khi cập nhật CCCD của sinh viên
create trigger trg_KTCapNhatSV
ON dbo.SinhVien
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Nếu CCCD bị NULL hoặc để trống
    IF EXISTS
    (
        SELECT 1
        FROM inserted
        WHERE CCCD IS NULL
           OR LTRIM(RTRIM(CCCD)) = ''
    )
    BEGIN
        -- Hủy thao tác UPDATE
        ROLLBACK TRANSACTION;

        -- Trả thông báo lỗi về cho ứng dụng C#
        THROW 50001,
              N'CCCD không được để trống!',
              1;
    END
END;
GO


-- Tạo trigger kiểm tra khi xóa sinh viên
create trigger trg_KTXoaSV
ON dbo.SinhVien
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Bảng deleted chứa dữ liệu của sinh viên
    -- ngay trước khi bản ghi bị xóa
    INSERT INTO dbo.LichSuXoaSinhVien
    (
        MaSV,
        HoTen,
        CCCD,
        MaLop,
        ThoiGianXoa
    )
    SELECT
        MaSV,
        HoTen,
        CCCD,
        MaLop,
        GETDATE()
    FROM deleted;
END;
GO


-- Tạo trigger kiểm tra khi xóa lớp
create trigger trg_KTXoaLop
on Lop
after delete
as
begin
    if exists
    (
        select * from deleted
    )
    begin
        print N'Đã xóa lớp thành công!';
    end
end;
go


-- Tạo trigger kiểm tra khi xóa ngành
create trigger trg_KTXoaNganh
on Nganh
instead of delete
as
begin
    if exists
    (
        select * from deleted
        where MaNganh in (select MaNganh from Lop)
    )
    begin
        print N'Không thể xóa ngành đang có lớp!';
    end
    else
    begin
        delete from Nganh
        where MaNganh in (select MaNganh from deleted);

        print N'Xóa ngành thành công!';
    end
end;
go


-- Tạo trigger kiểm tra khi xóa khoa
create trigger trg_KTXoaKhoa
on Khoa
instead of delete
as
begin
    if exists
    (
        select * from deleted
        where MaKhoa in (select MaKhoa from Nganh)
    )
    begin
        print N'Không thể xóa khoa đang có ngành!';
    end
    else
    begin
        delete from Khoa
        where MaKhoa in (select MaKhoa from deleted);

        print N'Xóa khoa thành công!';
    end
end;
go
