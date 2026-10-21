
-- ===================================
-- GIAO TÁC 
-- ===================================

-- Giao tác thêm sinh viên
begin transaction;
-- Kiểm tra mã sinh viên có bị trùng không
if exists
(
    select * from SinhVien
    where MaSV = 'SV15'
)
begin
    print N'Mã sinh viên đã tồn tại!';
    rollback transaction; 
end
else
begin
    insert into SinhVien
    values 
    ('SV15', N'Nguyễn Trúc Anh','075206005869','2006-05-10',
     N'Nữ','0978123456','anh@gmail.com',N'TP.HCM',
     N'Kinh', N'Đang học','L01');

    insert into NguoiThan
    values
    ('NT15',N'Lê Thị Hòa',N'Mẹ',N'Công nhân','0978125691',
     '1982-09-03',N'TP.HCM',N'Kinh',N'Nữ','SV15');

    commit transaction;
    print N'Thêm sinh viên thành công!';
end;
go

select * from SinhVien
go

select * from NguoiThan
go

-- Giao tác xóa sinh viên
begin transaction;
-- Kiểm tra mã sv có tồn tại không
if not exists 
(
    select * from SinhVien
    where MaSV = 'SV15'
)
begin 
    print N' Mã sinh viên không tồn tại!';
    rollback transaction;
end
else 
begin
    delete from NguoiThan
    where MaSV = 'SV15';

    delete from SinhVien
    where MaSV = 'SV15';

    commit transaction;
    print N'Xóa Sinh viên thành công!';
end;
go

select * from SinhVien;
go

select * from NguoiThan;
go

-- Giao tác chuyển lớp
begin transaction;
-- Ktra mã sv có trong lớp không 
if not exists
(
    select * from SinhVien
    where MaSV = 'SV10'
)
begin
    print N'Mã Sinh Viên không có tồn tại trong lớp !';
    rollback transaction;
end
else 
begin 
-- Ktra mã lớp này có trong sinh viên chưa
if exists 
(
    select * from SinhVien
    where MaSV ='SV10' and MaLop = 'L01'
)
begin 
    print N'Mã sinh viên đã tồn tại trong lớp này!'
    rollback transaction;
end 
else 
begin 
-- Cập nhật lớp mới 
    update SinhVien
    set MaLop ='L02'
    where MaSV ='SV10';

    commit transaction;
    print N'Chuyển lớp thành công!';
    end
end;
go

select * from SinhVien
go

-- Giao tác thêm sinh viên vào lớp
begin transaction;
-- Kiểm tra mã lớp có tồn tại không
if not exists
(
    select * from Lop
    where MaLop = 'L01'
)
begin
    print N'Mã lớp không tồn tại!';
    rollback transaction;
end
else
begin
    insert into SinhVien
    values
    ('SV14', N'Huỳnh Anh Kiệt', '086206005295', '2006-07-19',
     N'Nữ', '0979823456', 'kiet@gmail.com', N'Long An',
     N'Kinh', N'Đang học', null);

    update SinhVien
    set MaLop = 'L01'
    where MaSV = 'SV14';

    commit transaction;
    print N'Đã thhêm sinh viên vào lớp!';
end;
go

select * from SinhVien
go

-- Giao tác thêm khoa là thêm luôn ngành mới
begin transaction;
--Kiểm tra mã khoa có bị trùng không 
if exists 
(
    select * 
    from Khoa
    where MaKhoa = 'BCTT'
)
begin 
    print N'Mã khoa đã tồn tại!';
    rollback transaction;
end
else 
begin
-- Ktra mã ngành có bị trùng không
    if exists
    (
    select * 
    from Nganh
    where MaNganh = 'BC'
    )
    begin 
        print N' Mã ngành đã tồn tại!';
        rollback transaction;
    end
    else 
    begin 
        insert into Khoa
        values
        ('BCTT',N'Báo chí Truyền thông ','098641537','bctt@ut.edu.vn','TP.HCM');

        insert into Nganh
        values
        ('BC',N'Báo Chí','BCTT');

        commit transaction;
        print N'Thêm khoa và ngành thành công!';
    end;
end;
go

select * from Khoa
go

select * from Nganh
go

-- 3 cái Demo được trong transaction
--1. Transaction Lớp — chuyển toàn bộ SV sang lớp mới rồi xóa lớp cũ
    BEGIN TRANSACTION;

    -- Kiểm tra lớp cũ
    IF NOT EXISTS
    (
        SELECT 1
        FROM Lop
        WHERE MaLop = @MaLopCu
    )
    BEGIN
        THROW 50001, N'Lớp cũ không tồn tại!', 1;
    END;

    -- Kiểm tra lớp mới
    IF NOT EXISTS
    (
        SELECT 1
        FROM Lop
        WHERE MaLop = @MaLopMoi
    )
    BEGIN
        THROW 50002, N'Lớp mới không tồn tại!', 1;
    END;

    -- Không cho chọn trùng lớp
    IF @MaLopCu = @MaLopMoi
    BEGIN
        THROW 50003, N'Lớp cũ và lớp mới không được giống nhau!', 1;
    END;

    -- Chuyển toàn bộ sinh viên
    UPDATE SinhVien
    SET MaLop = @MaLopMoi
    WHERE MaLop = @MaLopCu;

    -- Xóa lớp cũ
    DELETE FROM Lop
    WHERE MaLop = @MaLopCu;

    COMMIT TRANSACTION;

    SELECT N'Chuyển sinh viên và xóa lớp thành công!' AS KetQua;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

--2. Transaction Khoa — thêm Khoa + Ngành đầu tiên
    BEGIN TRANSACTION;

    -- Kiểm tra mã khoa
    IF EXISTS
    (
        SELECT 1
        FROM Khoa
        WHERE MaKhoa = @MaKhoa
    )
    BEGIN
        THROW 50010, N'Mã khoa đã tồn tại!', 1;
    END;

    -- Kiểm tra mã ngành
    IF EXISTS
    (
        SELECT 1
        FROM Nganh
        WHERE MaNganh = @MaNganh
    )
    BEGIN
        THROW 50011, N'Mã ngành đã tồn tại!', 1;
    END;

    -- Thêm khoa
    INSERT INTO Khoa
    (
        MaKhoa,
        TenKhoa,
        SDT,
        Email,
        DiaChi
    )
    VALUES
    (
        @MaKhoa,
        @TenKhoa,
        @SDT,
        @Email,
        @DiaChi
    );

    -- Thêm ngành đầu tiên
    INSERT INTO Nganh
    (
        MaNganh,
        TenNganh,
        MaKhoa
    )
    VALUES
    (
        @MaNganh,
        @TenNganh,
        @MaKhoa
    );

    COMMIT TRANSACTION;

    SELECT N'Thêm khoa và ngành thành công!' AS KetQua;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

--3. Transaction Ngành — thêm Ngành + Chương trình đào tạo
    BEGIN TRANSACTION;

    -- Kiểm tra khoa
    IF NOT EXISTS
    (
        SELECT 1
        FROM Khoa
        WHERE MaKhoa = @MaKhoa
    )
    BEGIN
        THROW 50020, N'Khoa không tồn tại!', 1;
    END;

    -- Kiểm tra ngành
    IF EXISTS
    (
        SELECT 1
        FROM Nganh
        WHERE MaNganh = @MaNganh
    )
    BEGIN
        THROW 50021, N'Mã ngành đã tồn tại!', 1;
    END;

    -- Kiểm tra CTĐT
    IF EXISTS
    (
        SELECT 1
        FROM ChuongTrinhDaoTao
        WHERE MaCTDT = @MaCTDT
    )
    BEGIN
        THROW 50022, N'Mã chương trình đào tạo đã tồn tại!', 1;
    END;

    -- Thêm ngành
    INSERT INTO Nganh
    (
        MaNganh,
        TenNganh,
        MaKhoa
    )
    VALUES
    (
        @MaNganh,
        @TenNganh,
        @MaKhoa
    );

    -- Thêm chương trình đào tạo
    INSERT INTO ChuongTrinhDaoTao
    (
        MaCTDT,
        TenCTDT,
        TrinhDo,
        ThoiGianDaoTao,
        TongTinChi,
        NamBatDau,
        NamKetThuc,
        MaNganh
    )
    VALUES
    (
        @MaCTDT,
        @TenCTDT,
        @TrinhDo,
        @ThoiGianDaoTao,
        @TongTinChi,
        @NamBatDau,
        @NamKetThuc,
        @MaNganh
    );

    COMMIT TRANSACTION;

    SELECT N'Thêm ngành và chương trình đào tạo thành công!' AS KetQua;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

/* ============================================================
   DEMO 4 LỖI GIAO TÁC + 4 CÁCH KHẮC PHỤC
   UI CHỈ GỌI STORED PROCEDURE
   ============================================================ */

IF OBJECT_ID('dbo.DemoControl', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DemoControl
    (
        RunId UNIQUEIDENTIFIER PRIMARY KEY,
        Scenario VARCHAR(20) NOT NULL,
        Mode VARCHAR(10) NOT NULL,          -- ERROR / FIX
        T1Step INT NOT NULL DEFAULT 0,
        T2Step INT NOT NULL DEFAULT 0,
        T1Action VARCHAR(20) NULL,
        T2Action VARCHAR(20) NULL,
        Cancelled BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSDATETIME()
    );
END;
GO

IF OBJECT_ID('dbo.DemoLog', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DemoLog
    (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        RunId UNIQUEIDENTIFIER NOT NULL,
        GiaoTac VARCHAR(5) NOT NULL,
        Loai VARCHAR(20) NOT NULL,
        NoiDung NVARCHAR(500) NOT NULL,
        ThoiGian DATETIME2(3) NOT NULL DEFAULT SYSDATETIME()
    );
END;
GO

/* Index để SERIALIZABLE tạo range lock rõ ràng cho Phantom Read */
IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ChuongTrinhDaoTao_MaNganh'
      AND object_id = OBJECT_ID('dbo.ChuongTrinhDaoTao')
)
BEGIN
    CREATE INDEX IX_ChuongTrinhDaoTao_MaNganh
    ON dbo.ChuongTrinhDaoTao(MaNganh);
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Demo_Log
    @RunId UNIQUEIDENTIFIER,
    @GiaoTac VARCHAR(5),
    @Loai VARCHAR(20),
    @NoiDung NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.DemoLog(RunId, GiaoTac, Loai, NoiDung)
    VALUES(@RunId, @GiaoTac, @Loai, @NoiDung);
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Demo_GetLog
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, GiaoTac, Loai, NoiDung, ThoiGian
    FROM dbo.DemoLog WITH (NOLOCK)
    WHERE RunId = @RunId
    ORDER BY Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Demo_SetStep
    @RunId UNIQUEIDENTIFIER,
    @GiaoTac VARCHAR(5),
    @Step INT,
    @Action VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @GiaoTac = 'T1'
    BEGIN
        UPDATE dbo.DemoControl
        SET T1Step = @Step,
            T1Action = COALESCE(@Action, T1Action)
        WHERE RunId = @RunId;
    END
    ELSE
    BEGIN
        UPDATE dbo.DemoControl
        SET T2Step = @Step,
            T2Action = COALESCE(@Action, T2Action)
        WHERE RunId = @RunId;
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Demo_Cancel
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.DemoControl
    SET Cancelled = 1,
        T1Step = 99,
        T2Step = 99,
        T1Action = 'ROLLBACK',
        T2Action = 'ROLLBACK'
    WHERE RunId = @RunId;
END;
GO

/* ============================================================
   KHỞI TẠO / RESET DỮ LIỆU THEO KỊCH BẢN
   DR  = Dirty Read
   NRR = Non-repeatable Read
   LU  = Lost Update
   PR  = Phantom Read
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_Demo_Init
    @RunId UNIQUEIDENTIFIER,
    @Scenario VARCHAR(20),
    @Mode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.DemoControl WHERE RunId = @RunId;
    DELETE FROM dbo.DemoLog WHERE RunId = @RunId;

    IF @Scenario = 'DR'
    BEGIN
        UPDATE dbo.SinhVien
        SET TrangThaiHocTap = N'Đang học'
        WHERE MaSV = 'SV01';
    END;

    IF @Scenario = 'NRR'
    BEGIN
        UPDATE dbo.Khoa
        SET SDT = '0281234567'
        WHERE MaKhoa = 'CNTT';
    END;

    IF @Scenario = 'LU'
    BEGIN
        UPDATE dbo.NguoiThan
        SET SDT = '0901234568',
            DiaChi = N'Vũng Tàu'
        WHERE MaNT = 'NT01';
    END;

    IF @Scenario = 'PR'
    BEGIN
        DELETE FROM dbo.ChuongTrinhDaoTao
        WHERE MaCTDT = 'CT99';
    END;

    INSERT INTO dbo.DemoControl
    (
        RunId, Scenario, Mode,
        T1Step, T2Step,
        T1Action, T2Action, Cancelled
    )
    VALUES
    (
        @RunId, @Scenario, @Mode,
        0, 0, NULL, NULL, 0
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Demo_Inspector
    @Scenario VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Scenario = 'DR'
    BEGIN
        SELECT MaSV, HoTen, TrangThaiHocTap, MaLop
        FROM dbo.SinhVien WITH (NOLOCK)
        WHERE MaSV = 'SV01';
        RETURN;
    END;

    IF @Scenario = 'NRR'
    BEGIN
        SELECT MaKhoa, TenKhoa, SDT, Email, DiaChi
        FROM dbo.Khoa WITH (NOLOCK)
        WHERE MaKhoa = 'CNTT';
        RETURN;
    END;

    IF @Scenario = 'LU'
    BEGIN
        SELECT MaNT, HoTen, MoiQuanHe, SDT, DiaChi, MaSV
        FROM dbo.NguoiThan WITH (NOLOCK)
        WHERE MaNT = 'NT01';
        RETURN;
    END;

    IF @Scenario = 'PR'
    BEGIN
        SELECT MaCTDT, TenCTDT, TrinhDo, TongTinChi, MaNganh
        FROM dbo.ChuongTrinhDaoTao WITH (NOLOCK)
        WHERE MaNganh = 'HHTT'
        ORDER BY MaCTDT;
        RETURN;
    END;
END;
GO

/* ============================================================
   1) DIRTY READ
   LỖI: READ UNCOMMITTED đọc dữ liệu T1 chưa COMMIT
   FIX: READ COMMITTED chờ T1 COMMIT/ROLLBACK
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_DR_T1_Runner
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Step INT = 0, @Cancelled BIT = 0, @Action VARCHAR(20);

    /* Chờ nút 1 */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1 RETURN;
        IF @Step >= 1 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    BEGIN TRANSACTION;

    EXEC dbo.sp_Demo_Log @RunId, 'T1', 'INFO',
        N'① BEGIN TRANSACTION - Phòng Đào tạo bắt đầu giao tác';

    /* Chờ nút 2 */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Step >= 2 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    UPDATE dbo.SinhVien
    SET TrangThaiHocTap = N'Bảo lưu'
    WHERE MaSV = 'SV01';

    EXEC dbo.sp_Demo_Log @RunId, 'T1', 'DANGER',
        N'② UPDATE SV01: Đang học → Bảo lưu (CHƯA COMMIT)';

    /* Chờ COMMIT / ROLLBACK */
    WHILE 1 = 1
    BEGIN
        SELECT @Action = T1Action, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Action IN ('COMMIT', 'ROLLBACK') BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    IF @Action = 'COMMIT'
    BEGIN
        COMMIT TRANSACTION;
        EXEC dbo.sp_Demo_Log @RunId, 'T1', 'SUCCESS',
            N'③ COMMIT - Bảo lưu trở thành dữ liệu chính thức';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        EXEC dbo.sp_Demo_Log @RunId, 'T1', 'SUCCESS',
            N'③ ROLLBACK - SV01 trở lại Đang học';
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DR_T2_Read
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Mode VARCHAR(10), @TrangThai NVARCHAR(50),
            @Msg NVARCHAR(500);

    SELECT @Mode = Mode
    FROM dbo.DemoControl WITH (NOLOCK)
    WHERE RunId = @RunId;

    IF @Mode = 'ERROR'
    BEGIN
        EXEC dbo.sp_Demo_Log @RunId, 'T2', 'DANGER',
            N'① T2 dùng READ UNCOMMITTED - có thể đọc dữ liệu chưa COMMIT';

        SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

        SELECT @TrangThai = TrangThaiHocTap
        FROM dbo.SinhVien
        WHERE MaSV = 'SV01';

        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
    END
    ELSE
    BEGIN
        EXEC dbo.sp_Demo_Log @RunId, 'T2', 'WAIT',
            N'① T2 dùng READ COMMITTED - đang chờ T1 kết thúc nếu còn khóa';

        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

        SELECT @TrangThai = TrangThaiHocTap
        FROM dbo.SinhVien
        WHERE MaSV = 'SV01';
    END;

    SET @Msg =
        N'② T2 đọc được TrangThaiHocTap = ' +
        ISNULL(@TrangThai, N'NULL');

    EXEC dbo.sp_Demo_Log
        @RunId, 'T2', 'SUCCESS', @Msg;

    SELECT
        CASE WHEN @Mode = 'ERROR'
             THEN N'READ UNCOMMITTED'
             ELSE N'READ COMMITTED'
        END AS MucCoLap,
        MaSV, HoTen, TrangThaiHocTap, MaLop
    FROM dbo.SinhVien WITH (NOLOCK)
    WHERE MaSV = 'SV01';
END;
GO

/* ============================================================
   2) NON-REPEATABLE READ
   LỖI: READ COMMITTED -> T2 chen UPDATE giữa hai lần đọc
   FIX: REPEATABLE READ -> giữ shared lock tới COMMIT
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_NRR_T1_Runner
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Mode VARCHAR(10), @Step INT = 0, @Cancelled BIT = 0,
            @Action VARCHAR(20), @Lan1 VARCHAR(20), @Lan2 VARCHAR(20),
            @Msg NVARCHAR(500), @LogType VARCHAR(20);

    SELECT @Mode = Mode
    FROM dbo.DemoControl WITH (NOLOCK)
    WHERE RunId = @RunId;

    /* Chờ BEGIN */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1 RETURN;
        IF @Step >= 1 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    IF @Mode = 'FIX'
        SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
    ELSE
        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRANSACTION;

    SET @Msg =
        CASE WHEN @Mode = 'FIX'
             THEN N'① BEGIN TRAN - REPEATABLE READ'
             ELSE N'① BEGIN TRAN - READ COMMITTED'
        END;

    EXEC dbo.sp_Demo_Log
        @RunId, 'T1', 'INFO', @Msg;

    /* Đọc lần 1 */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Step >= 2 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    SELECT @Lan1 = SDT
    FROM dbo.Khoa
    WHERE MaKhoa = 'CNTT';

    SET @Msg =
        N'② T1 đọc SDT lần 1 = ' +
        ISNULL(@Lan1, 'NULL');

    EXEC dbo.sp_Demo_Log
        @RunId, 'T1', 'INFO', @Msg;

    /* Đọc lần 2 */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Step >= 3 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    SELECT @Lan2 = SDT
    FROM dbo.Khoa
    WHERE MaKhoa = 'CNTT';

    SET @LogType =
        CASE WHEN @Lan1 <> @Lan2
             THEN 'DANGER'
             ELSE 'SUCCESS'
        END;

    SET @Msg =
        N'③ T1 đọc SDT lần 2 = ' +
        ISNULL(@Lan2, 'NULL');

    EXEC dbo.sp_Demo_Log
        @RunId, 'T1', @LogType, @Msg;

    /* Chờ COMMIT */
    WHILE 1 = 1
    BEGIN
        SELECT @Action = T1Action, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Action = 'COMMIT' BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    COMMIT TRANSACTION;

    EXEC dbo.sp_Demo_Log @RunId, 'T1', 'SUCCESS',
        N'④ T1 COMMIT';

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_NRR_T2_Update
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.sp_Demo_Log @RunId, 'T2', 'WAIT',
        N'① T2 yêu cầu UPDATE SDT CNTT → 0289999888';

    BEGIN TRANSACTION;

    UPDATE dbo.Khoa
    SET SDT = '0289999888'
    WHERE MaKhoa = 'CNTT';

    COMMIT TRANSACTION;

    EXEC dbo.sp_Demo_Log @RunId, 'T2', 'SUCCESS',
        N'② T2 UPDATE + COMMIT thành công';

    SELECT MaKhoa, TenKhoa, SDT
    FROM dbo.Khoa
    WHERE MaKhoa = 'CNTT';
END;
GO

/* ============================================================
   3) LOST UPDATE
   LỖI: cả hai cùng đọc bản cũ, T2 lưu sau ghi đè SDT mới của T1
   FIX: UPDLOCK giữ khóa cập nhật từ bước đọc
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_LU_T1_Runner
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Mode VARCHAR(10), @Step INT = 0, @Cancelled BIT = 0,
            @OldSDT VARCHAR(20), @OldDiaChi NVARCHAR(100),
            @Msg NVARCHAR(500), @LogType VARCHAR(20);

    SELECT @Mode = Mode
    FROM dbo.DemoControl WITH (NOLOCK)
    WHERE RunId = @RunId;

    /* Chờ mở dữ liệu */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1 RETURN;
        IF @Step >= 1 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    BEGIN TRANSACTION;

    IF @Mode = 'FIX'
    BEGIN
        SELECT @OldSDT = SDT, @OldDiaChi = DiaChi
        FROM dbo.NguoiThan WITH (UPDLOCK, ROWLOCK)
        WHERE MaNT = 'NT01';
    END
    ELSE
    BEGIN
        SELECT @OldSDT = SDT, @OldDiaChi = DiaChi
        FROM dbo.NguoiThan
        WHERE MaNT = 'NT01';
    END;

    SET @Msg =
        N'① T1 mở NT01, SDT đang thấy = ' +
        ISNULL(@OldSDT, 'NULL');

    EXEC dbo.sp_Demo_Log
        @RunId, 'T1', 'INFO', @Msg;

    /* Chờ T1 lưu SDT */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Step >= 2 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    UPDATE dbo.NguoiThan
    SET SDT = '0901234111'
    WHERE MaNT = 'NT01';

    COMMIT TRANSACTION;

    EXEC dbo.sp_Demo_Log @RunId, 'T1', 'SUCCESS',
        N'② T1 lưu SDT mới 0901234111 và COMMIT';
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LU_T2_Runner
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Mode VARCHAR(10), @Step INT = 0, @Cancelled BIT = 0,
            @OldSDT VARCHAR(20), @OldDiaChi NVARCHAR(100),
            @Msg NVARCHAR(500), @LogType VARCHAR(20);

    SELECT @Mode = Mode
    FROM dbo.DemoControl WITH (NOLOCK)
    WHERE RunId = @RunId;

    /* Chờ T2 mở dữ liệu */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T2Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1 RETURN;
        IF @Step >= 1 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    BEGIN TRANSACTION;

    IF @Mode = 'FIX'
    BEGIN
        EXEC dbo.sp_Demo_Log @RunId, 'T2', 'WAIT',
            N'① T2 mở NT01 bằng UPDLOCK - sẽ chờ nếu T1 đang giữ khóa';

        SELECT @OldSDT = SDT, @OldDiaChi = DiaChi
        FROM dbo.NguoiThan WITH (UPDLOCK, ROWLOCK)
        WHERE MaNT = 'NT01';
    END
    ELSE
    BEGIN
        SELECT @OldSDT = SDT, @OldDiaChi = DiaChi
        FROM dbo.NguoiThan
        WHERE MaNT = 'NT01';
    END;

    SET @Msg =
        N'② T2 đang giữ bản dữ liệu có SDT = ' +
        ISNULL(@OldSDT, 'NULL');

    EXEC dbo.sp_Demo_Log
        @RunId, 'T2', 'INFO', @Msg;

    /* Chờ T2 lưu địa chỉ */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T2Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Step >= 2 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    /* Mô phỏng form lưu toàn bộ dòng từ dữ liệu đã mở trước đó */
    UPDATE dbo.NguoiThan
    SET SDT = @OldSDT,
        DiaChi = N'Bình Dương'
    WHERE MaNT = 'NT01';

    COMMIT TRANSACTION;

    SET @LogType =
        CASE WHEN @OldSDT = '0901234568'
             THEN 'DANGER'
             ELSE 'SUCCESS'
        END;

    SET @Msg =
        N'③ T2 lưu địa chỉ Bình Dương + SDT đang giữ = ' +
        ISNULL(@OldSDT, 'NULL');

    EXEC dbo.sp_Demo_Log
        @RunId, 'T2', @LogType, @Msg;

    SELECT MaNT, HoTen, SDT, DiaChi
    FROM dbo.NguoiThan
    WHERE MaNT = 'NT01';
END;
GO

/* ============================================================
   4) PHANTOM READ
   LỖI: READ COMMITTED -> T2 INSERT thêm dòng giữa hai lần đọc
   FIX: SERIALIZABLE -> range lock chặn INSERT
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_PR_T1_Runner
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Mode VARCHAR(10), @Step INT = 0, @Cancelled BIT = 0,
            @Action VARCHAR(20), @Lan1 INT, @Lan2 INT,
            @Msg NVARCHAR(500), @LogType VARCHAR(20);

    SELECT @Mode = Mode
    FROM dbo.DemoControl WITH (NOLOCK)
    WHERE RunId = @RunId;

    /* Chờ BEGIN */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1 RETURN;
        IF @Step >= 1 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    IF @Mode = 'FIX'
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    ELSE
        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRANSACTION;

    SET @Msg =
        CASE WHEN @Mode = 'FIX'
             THEN N'① BEGIN TRAN - SERIALIZABLE'
             ELSE N'① BEGIN TRAN - READ COMMITTED'
        END;

    EXEC dbo.sp_Demo_Log
        @RunId, 'T1', 'INFO', @Msg;

    /* Đọc lần 1 */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Step >= 2 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    SELECT @Lan1 = COUNT(*)
    FROM dbo.ChuongTrinhDaoTao
    WHERE MaNganh = 'HHTT';

    SET @Msg =
        N'② T1 đếm lần 1 = ' +
        CAST(@Lan1 AS NVARCHAR(20)) +
        N' CTĐT ngành HHTT';

    EXEC dbo.sp_Demo_Log
        @RunId, 'T1', 'INFO', @Msg;

    /* Đọc lần 2 */
    WHILE 1 = 1
    BEGIN
        SELECT @Step = T1Step, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Step >= 3 BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    SELECT @Lan2 = COUNT(*)
    FROM dbo.ChuongTrinhDaoTao
    WHERE MaNganh = 'HHTT';

    SET @LogType =
        CASE WHEN @Lan1 <> @Lan2
             THEN 'DANGER'
             ELSE 'SUCCESS'
        END;

    SET @Msg =
        N'③ T1 đếm lần 2 = ' +
        CAST(@Lan2 AS NVARCHAR(20)) +
        N' CTĐT ngành HHTT';

    EXEC dbo.sp_Demo_Log
        @RunId, 'T1', @LogType, @Msg;

    /* Chờ COMMIT */
    WHILE 1 = 1
    BEGIN
        SELECT @Action = T1Action, @Cancelled = Cancelled
        FROM dbo.DemoControl WITH (NOLOCK)
        WHERE RunId = @RunId;

        IF @Cancelled = 1
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @Action = 'COMMIT' BREAK;
        WAITFOR DELAY '00:00:00.150';
    END;

    COMMIT TRANSACTION;

    EXEC dbo.sp_Demo_Log @RunId, 'T1', 'SUCCESS',
        N'④ T1 COMMIT';

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PR_T2_Insert
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.sp_Demo_Log @RunId, 'T2', 'WAIT',
        N'① T2 yêu cầu INSERT CT99 vào ngành HHTT';

    BEGIN TRANSACTION;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.ChuongTrinhDaoTao
        WHERE MaCTDT = 'CT99'
    )
    BEGIN
        INSERT INTO dbo.ChuongTrinhDaoTao
        (
            MaCTDT, TenCTDT, TrinhDo,
            ThoiGianDaoTao, TongTinChi,
            NamBatDau, NamKetThuc, MaNganh
        )
        VALUES
        (
            'CT99', N'Chương trình Demo Phantom',
            N'Đại học', 4, 120, 2026, 2030, 'HHTT'
        );
    END;

    COMMIT TRANSACTION;

    EXEC dbo.sp_Demo_Log @RunId, 'T2', 'SUCCESS',
        N'② T2 INSERT CT99 + COMMIT thành công';

    SELECT MaCTDT, TenCTDT, MaNganh
    FROM dbo.ChuongTrinhDaoTao
    WHERE MaCTDT = 'CT99';
END;
GO
