using System;
using System.Data;
using Microsoft.Data.SqlClient;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL
{
    public class LopDAL : QLHSSV_Nhóm8.DAL.Interfaces.ILopDAL
    {
        // Lấy danh sách lớp từ View
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT
                        MaLop,
                        TenLop,
                        NienKhoa,
                        MaNganh
                    FROM dbo.vw_LopDayDu
                    ORDER BY MaLop";

                using (SqlDataAdapter da =
                       new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // Kiểm tra mã lớp tồn tại
        public bool KiemTraTonTai(string maLop)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_KiemTraLopTonTai",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaLop",
                        SqlDbType.VarChar,
                        10
                    ).Value = maLop;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Thêm lớp bằng Stored Procedure
        public bool Insert(LopDTO lop)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_ThemLop",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, lop);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        // Sửa lớp bằng Stored Procedure
        public bool Update(LopDTO lop)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_SuaLop",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, lop);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        // Xóa lớp bằng Stored Procedure
        public bool Delete(string maLop)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_XoaLop",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaLop",
                        SqlDbType.VarChar,
                        10
                    ).Value = maLop;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Tìm lớp bằng Stored Procedure
        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_TimLop",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@TuKhoa",
                        SqlDbType.NVarChar,
                        100
                    ).Value = keyword;

                    using (SqlDataAdapter da =
                           new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // Transaction: chuyển SV sang lớp mới rồi xóa lớp cũ
        public string ChuyenSinhVienVaXoaLop(
            string maLopCu,
            string maLopMoi)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql = @"
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF @MaLopCu = @MaLopMoi
        THROW 50001, N'Lớp cũ và lớp mới không được giống nhau!', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Lop
        WHERE MaLop = @MaLopCu
    )
        THROW 50002, N'Lớp cũ không tồn tại!', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Lop
        WHERE MaLop = @MaLopMoi
    )
        THROW 50003, N'Lớp mới không tồn tại!', 1;

    UPDATE SinhVien
    SET MaLop = @MaLopMoi
    WHERE MaLop = @MaLopCu;

    DECLARE @SoSinhVien INT = @@ROWCOUNT;

    DELETE FROM Lop
    WHERE MaLop = @MaLopCu;

    COMMIT TRANSACTION;

    SELECT
        N'Đã chuyển ' +
        CAST(@SoSinhVien AS NVARCHAR(10)) +
        N' sinh viên sang lớp ' +
        @MaLopMoi +
        N' và xóa lớp ' +
        @MaLopCu +
        N' thành công!';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;";

                using (SqlCommand cmd =
                       new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@MaLopCu",
                        SqlDbType.VarChar,
                        10
                    ).Value = maLopCu;

                    cmd.Parameters.Add(
                        "@MaLopMoi",
                        SqlDbType.VarChar,
                        10
                    ).Value = maLopMoi;

                    object? result =
                        cmd.ExecuteScalar();

                    return result?.ToString()
                           ?? "Giao tác hoàn tất.";
                }
            }
        }

        // Gán tham số Thêm/Sửa
        private void GanThamSo(
            SqlCommand cmd,
            LopDTO lop)
        {
            cmd.Parameters.Add(
                "@MaLop",
                SqlDbType.VarChar,
                10
            ).Value = lop.MaLop;

            cmd.Parameters.Add(
                "@TenLop",
                SqlDbType.NVarChar,
                50
            ).Value = lop.TenLop;

            cmd.Parameters.Add(
                "@NienKhoa",
                SqlDbType.NVarChar,
                10
            ).Value = lop.NienKhoa;

            cmd.Parameters.Add(
                "@MaNganh",
                SqlDbType.VarChar,
                10
            ).Value = lop.MaNganh;
        }
    }
}
