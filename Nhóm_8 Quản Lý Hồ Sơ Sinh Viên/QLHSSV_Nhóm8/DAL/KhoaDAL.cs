using System;
using System.Data;
using Microsoft.Data.SqlClient;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL
{
    public class KhoaDAL : QLHSSV_Nhóm8.DAL.Interfaces.IKhoaDAL
    {
        // Lấy danh sách khoa từ View
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT
                        MaKhoa,
                        TenKhoa,
                        SDT,
                        Email,
                        DiaChi
                    FROM dbo.vw_KhoaDayDu
                    ORDER BY MaKhoa";

                using (SqlDataAdapter da =
                       new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // Kiểm tra mã khoa tồn tại
        public bool KiemTraTonTai(string maKhoa)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_KiemTraKhoaTonTai",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaKhoa",
                        SqlDbType.VarChar,
                        10
                    ).Value = maKhoa;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Thêm khoa
        public bool Insert(KhoaDTO khoa)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_ThemKhoa",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, khoa);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Sửa khoa
        public bool Update(KhoaDTO khoa)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_SuaKhoa",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, khoa);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Xóa khoa
        public bool Delete(string maKhoa)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_XoaKhoa",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaKhoa",
                        SqlDbType.VarChar,
                        10
                    ).Value = maKhoa;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Tìm khoa
        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_TimKhoa",
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
            KhoaDTO khoa)
        {
            cmd.Parameters.Add(
                "@MaKhoa",
                SqlDbType.VarChar,
                10
            ).Value = khoa.MaKhoa;

            cmd.Parameters.Add(
                "@TenKhoa",
                SqlDbType.NVarChar,
                50
            ).Value = khoa.TenKhoa;

            cmd.Parameters.Add(
                "@SDT",
                SqlDbType.VarChar,
                20
            ).Value = khoa.SDT;

            cmd.Parameters.Add(
                "@Email",
                SqlDbType.VarChar,
                50
            ).Value = khoa.Email;

            cmd.Parameters.Add(
                "@DiaChi",
                SqlDbType.NVarChar,
                100
            ).Value = khoa.DiaChi;
        }
    }
}
