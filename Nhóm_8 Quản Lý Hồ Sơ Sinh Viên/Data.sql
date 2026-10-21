create database QLHSSV;
go
use QLHSSV;
go
create table TaiKhoan
(
    TenDangNhap nvarchar(50),
    MatKhau varchar(100) Not Null,
    VaiTro nvarchar(20) Not Null,
    TrangThai bit Not Null Default 1);
go

insert into dbo.TaiKhoan
( TenDangNhap,MatKhau,VaiTro,TrangThai)
Values
('admin','123456',N'Admin',1),
('staff','123456',N'Nhan Vien',1);
go

CREATE TABLE dbo.LichSuXoaSinhVien
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,

        -- Thông tin sinh viên trước khi bị xóa
        MaSV VARCHAR(10),
        HoTen NVARCHAR(50),
        CCCD VARCHAR(20),
        MaLop VARCHAR(10),

        -- Thời điểm Trigger ghi nhận việc xóa
        ThoiGianXoa DATETIME
            NOT NULL DEFAULT GETDATE()
    );
GO

create table Khoa(
	MaKhoa varchar (10) primary key,
	TenKhoa nvarchar(50),
    SDT varchar(20),
    Email varchar(50),
    DiaChi nvarchar(100)
);
go 

insert into Khoa
values 
('KT', N'Kỹ thuật', '0123456789', 'kt@uth.edu.vn', N'TP.HCM'),
('XD', N'Xây Dựng', '0123456788', 'xd@uth.edu.vn', N'TP.HCM'),
('NN', N'Ngôn Ngữ', '0123456787', 'nn@uth.edu.vn', N'TP.HCM'),
('HH', N'Hàng Hải', '0123456786', 'hh@uth.edu.vn', N'TP.HCM'),
('CNTT', N'Công nghệ thông tin', '0123456785','cntt@uth.edu.vn', N'TP.HCM'),
('TM', N'Thương mại', '0123456784', 'tm@uth.edu.vn', N'TP.HCM');
go

select * from Khoa;
go

create table Nganh(
    MaNganh varchar(10) primary key,
    TenNganh nvarchar(50),
    MaKhoa varchar(10),
    foreign key (MaKhoa) references Khoa(MaKhoa)
);
go

insert into Nganh
values
('KTXD', N'Kỹ thuật xây dựng', 'XD'),
('KTDT', N'Kỹ thuật điện tử', 'KT'),
('NNA', N'Ngôn ngữ Anh', 'NN'),
('KTVT', N'Kinh tế vận tải', 'HH'),
('HHTT', N'Hệ thống thông tin', 'CNTT'),
('DM', N'Digital Marketting', 'TM');
go

select * from Nganh;
go

create table Lop(
    MaLop varchar(10) primary key,
    TenLop nvarchar(50),
    NienKhoa nvarchar(10),
    MaNganh varchar(10),
    foreign key (MaNganh) references Nganh(MaNganh)
);
go

insert into Lop
values
('L01', N'Kỹ thuật xây dựng 1', N'2025-2026', 'KTXD'),
('L02', N'Kỹ thuật xây dựng 2', N'2025-2026', 'KTXD'),
('L03', N'Kỹ thuật điện tử 1', N'2025-2026', 'KTDT'),
('L04', N'Ngôn ngữ Anh 1', N'2025-2026', 'NNA'),
('L05', N'Ngôn ngữ Anh 2', N'2025-2026', 'NNA'),
('L06', N'Kinh tế vận tải 2', N'2025-2026', 'KTVT'),
('L07', N'Hệ thống thông tin 1', N'2025-2026', 'HHTT'),
('L08', N'Hệ thống thông tin 2', N'2025-2026', 'HHTT'),
('L09', N'Điện toán đám mây 1', N'2025-2026', 'HHTT'),
('L10', N'Điện toán đám mây 2', N'2025-2026', 'HHTT');
go

select * from Lop;
go

create table SinhVien(
    MaSV varchar(10) primary key,
    HoTen nvarchar(50),
    CCCD varchar(20),
    NgaySinh date,
    GioiTinh nvarchar(10),
    SDT varchar(20),
    Email varchar(50),
    DiaChi nvarchar(100),
    DanToc nvarchar(20),
    TrangThaiHocTap nvarchar(50),
    MaLop varchar(10),
    foreign key (MaLop) references Lop(MaLop)
);
go

insert into SinhVien
values
('SV01', N'Nguyễn Văn An', '079123456789', '2006-01-28', N'Nam', '0901234567', 'an@gmail.com', N'Vũng Tàu', N'Kinh', N'Đang học', 'L01'),
('SV02', N'Trần Thị Hoa', '078123456788', '2006-03-15', N'Nữ', '0901234568', 'hoa@gmail.com', N'TP.HCM', N'Kinh', N'Đang học', 'L01'),
('SV03', N'Lê Chí Nguyên ', '077123456787', '2006-06-29', N'Nam', '0901234569', 'nguyen@gmail.com', N'Vĩnh Long', N'Khơ me', N'Đang học', 'L02'),
('SV04', N'Phạm Bảo Ngọc', '076123456786', '2006-07-10', N'Nữ', '0901234570', 'ngoc@gmail.com', N'Long An', N'Hoa', N'Đang học', 'L03'),
('SV05', N'Nguyễn Thành Đạt', '075123456785', '2006-11-25', N'Nam', '0901234571', 'dat@gmail.com', N'Tiền Giang', N'Kinh', N'Đang học', 'L04'),
('SV06', N'Võ Kim Ngân ', '074123456784', '2006-09-12', N'Nữ', '0901234572', 'ngan@gmail.com', N'Tây Ninh', N'Kinh', N'Đang học', 'L05'),
('SV07', N'Trần Văn Khang', '073123456783', '2006-02-18', N'Nam', '0901234573', 'khang@gmail.com', N'TP.HCM', N'Kinh', N'Đang học', 'L06'),
('SV08', N'Đỗ Thị Tuyết Lan', '072123456782', '2006-04-22', N'Nữ', '0901234574', 'lan@gmail.com', N'Đồng Tháp', N'Kinh', N'Đang học', 'L07'),
('SV09', N'Phan Tiến Minh', '071123456781', '2005-12-05', N'Nam', '0901234575', 'minh@gmail.com', N'Đồng Nai', N'Hoa', N'Đang học', 'L08'),
('SV10', N'Hoàng Yến Nhi', '070123456780', '2006-06-30', N'Nữ', '0901234576', 'nhi@gmail.com', N'Bến Tre', N'Khơ me', N'Đang học', 'L09');
go

select * from SinhVien;
go

create table NguoiThan(
    MaNT varchar(10) primary key,
    HoTen nvarchar(50),
    MoiQuanHe nvarchar(20),
    NgheNghiep nvarchar(20),
    SDT varchar(20),
    NgaySinh date,
    DiaChi nvarchar(100),
    DanToc nvarchar(20),
    GioiTinh nvarchar(10),
    MaSV varchar(10),
    foreign key (MaSV) references SinhVien(MaSV)
);
go

insert into NguoiThan
values
('NT01', N'Nguyễn Thị Hương', N'Mẹ', N'Kinh doanh', '0901234567', '1975-05-10', N'Vũng Tàu', N'Kinh', N'Nữ', 'SV01'),
('NT02', N'Trần Văn Hùng', N'Ba', N'Kỹ sư', '0901234568', '1976-08-20', N'TP.HCM', N'Kinh', N'Nam', 'SV02'),
('NT03', N'Lê Văn Thành', N'Ba', N'Buôn bán', '0901234569', '1974-03-15', N'Vĩnh Long', N'Kinh', N'Nam', 'SV03'),
('NT04', N'Phạm Thị Mai', N'Mẹ', N'Giáo viên', '0901234570', '1978-06-18', N'Long An', N'Hoa', N'Nữ', 'SV04'),
('NT05', N'Nguyễn Văn Bình', N'Ba', N'Kinh doanh', '0901234571', '1975-10-12', N'Tiền Giang', N'Kinh', N'Nam', 'SV05'),
('NT06', N'Võ Văn Nam', N'Ba', N'Lái xe', '0901234572', '1977-03-22', N'Tây Ninh', N'Kinh', N'Nam', 'SV06'),
('NT07', N'Trần Văn Phúc', N'Ba', N'Kinh doanh', '0901234573', '1973-11-08', N'TP.HCM', N'Kinh', N'Nam', 'SV07'),
('NT08', N'Lê Thị Khánh', N'Mẹ', N'Giáo viên', '0901234574', '1976-04-25', N'Đồng Tháp', N'Kinh', N'Nữ', 'SV08'),
('NT09', N'Đặng Thu Hòa', N'Mẹ', N'Buôn bán', '0901234575', '1974-09-14', N'Đồng Nai', N'Hoa', N'Nữ', 'SV09'),
('NT10', N'Hoàng Văn Sơn', N'Ba', N'Kinh doanh', '0901234576', '1975-12-30', N'Bến Tre', N'Khơ me', N'Nam', 'SV10');
go

select * from NguoiThan;
go

create table ChuongTrinhDaoTao(
    MaCTDT varchar(10) primary key,
    TenCTDT nvarchar(50),
    TrinhDo nvarchar(20),
    ThoiGianDaoTao int,
    TongTinChi int,
    NamBatDau int,
    NamKetThuc int,
    MaNganh varchar(10),
    foreign key (MaNganh) references Nganh(MaNganh)
);
go

insert into ChuongTrinhDaoTao
values
('CT01', N'Kỹ thuật xây dựng', N'Đại học', 4, 120, 2024, 2028, 'KTXD'),
('CT02', N'Kỹ thuật điện tử', N'Đại học', 4, 120, 2024, 2028, 'KTDT'),
('CT03', N'Ngôn ngữ Anh', N'Đại học', 4, 120, 2024, 2028, 'NNA'),
('CT04', N'Kinh tế vận tải', N'Đại học', 4, 120, 2024, 2028, 'KTVT'),
('CT05', N'Hệ thống thông tin', N'Đại học', 4, 120, 2024, 2028, 'HHTT'),
('CT06', N'Digital Marketting', N'Đại học', 4, 120, 2024, 2028, 'DM');
go

select * from ChuongTrinhDaoTao;
go