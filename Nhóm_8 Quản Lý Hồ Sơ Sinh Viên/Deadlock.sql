use QLHSSV;
go

----------------------------------
-- DEADLOCK 
----------------------------------

----------------
-- T1. KHÓA LỚP 
----------------
begin transaction;

-- Khóa Lớp 'L01'
update Lop 
set TenLop = N' An ninh mạng' 
where MaLop = 'L01';

-- Giữ khóa 10 giây 
waitfor delay '00:00:10'; 

-- Sau đó T1 khóa 'SV01' để cập nhật trạng thái
update SinhVien 
set TrangThaiHocTap = N' Bảo lưu'
where MaSV = 'SV01';

commit transaction;

----------------------
-- T2. KHÓA SINH VIÊN 
----------------------
begin transaction;

-- Khóa Sinh Viên 'SV01'
update SinhVien 
set HoTen = N' Đặng Thành An'
where MaSV = 'SV01';

-- Giữ khóa 10 giây 
waitfor delay '00:00:10'; 

-- Sau đó T2 khóa 'L01' để điều chỉnh niên khóa
update Lop 
set NienKhoa = N'2025-2028'
where MaLop = 'L01';

commit transaction;

----------------------------------------
-- NGĂN NGỪA DEADLOCK THEO GIAO THỨC 
-- Ordering All the Items Protocol
----------------------------------------

----------------
-- T1. KHÓA LỚP 
----------------
begin transaction;

-- Khóa Lớp 'L01'
update Lop 
set TenLop = N' An ninh mạng' 
where MaLop = 'L01';

-- Giữ khóa 10 giây 
waitfor delay '00:00:10'; 

-- Sau đó mới khóa SinhVien 'SV01' để cập nhật trạng thái
update SinhVien 
set TrangThaiHocTap = N' Bảo lưu' 
where MaSV = 'SV01';

commit transaction;

----------------------
-- T2. KHÓA SINH VIÊN 
----------------------
begin transaction;

-- T2 cũng phải khóa Lớp 'L01' trước
update Lop
set NienKhoa = N'2025-2028'
where MaLop = 'L01';

-- Giữ khóa 10 giây
waitfor delay '00:00:10';

-- Sau đó mới khóa SinhVien 'SV01' để điều chỉnh họ tên 
update SinhVien
set HoTen = N' Đặng Thành An'
where MaSV = 'SV01';

commit transaction;

