using System;
using System.Data;
using Microsoft.Data.SqlClient;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL
{
    public class NganhDAL : QLHSSV_Nhóm8.DAL.Interfaces.INganhDAL
    {
        // Lấy danh sách ngành từ View
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT
                        MaNganh,
                        TenNganh,
                        MaKhoa
                    FROM dbo.vw_NganhDayDu
                    ORDER BY MaNganh";

                using (SqlDataAdapter da =
                       new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // Kiểm tra mã ngành tồn tại
        public bool KiemTraTonTai(string maNganh)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "sp_KiemTraNganhTonTai",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaNganh",
                        SqlDbType.VarChar,
                        10
                    ).Value = maNganh;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Thêm ngành bằng Stored Procedure
        public bool Insert(NganhDTO nganh)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_ThemNganh",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, nganh);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Sửa ngành bằng Stored Procedure
        public bool Update(NganhDTO nganh)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_SuaNganh",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, nganh);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Xóa ngành bằng Stored Procedure
        public bool Delete(string maNganh)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_XoaNganh",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaNganh",
                        SqlDbType.VarChar,
                        10
                    ).Value = maNganh;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Tìm ngành bằng Stored Procedure
        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_TimNganh",
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

        // Gán tham số Thêm/Sửa
        private void GanThamSo(
            SqlCommand cmd,
            NganhDTO nganh)
        {
            cmd.Parameters.Add(
                "@MaNganh",
                SqlDbType.VarChar,
                10
            ).Value = nganh.MaNganh;

            cmd.Parameters.Add(
                "@TenNganh",
                SqlDbType.NVarChar,
                50
            ).Value = nganh.TenNganh;

            cmd.Parameters.Add(
                "@MaKhoa",
                SqlDbType.VarChar,
                10
            ).Value = nganh.MaKhoa;
        }
    }
}
