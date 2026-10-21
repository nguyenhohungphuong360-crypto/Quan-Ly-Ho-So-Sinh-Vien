using System;
using System.Data;
using Microsoft.Data.SqlClient;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL
{
    public class ChuongTrinhDaoTaoDAL
    {
        // Lấy danh sách CTĐT từ View
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT
                        MaCTDT,
                        TenCTDT,
                        TrinhDo,
                        ThoiGianDaoTao,
                        TongTinChi,
                        NamBatDau,
                        NamKetThuc,
                        MaNganh
                    FROM dbo.vw_CTDTDayDu
                    ORDER BY MaCTDT";

                using (SqlDataAdapter da =
                       new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // Kiểm tra mã CTĐT tồn tại
        public bool KiemTraTonTai(string maCTDT)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_KiemTraCTDTTonTai",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaCTDT",
                        SqlDbType.VarChar,
                        10
                    ).Value = maCTDT;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Thêm CTĐT
        public bool Insert(ChuongTrinhDaoTaoDTO ct)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_ThemCTDT",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, ct);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Sửa CTĐT
        public bool Update(ChuongTrinhDaoTaoDTO ct)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_SuaCTDT",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, ct);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Xóa CTĐT
        public bool Delete(string maCTDT)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_XoaCTDT",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaCTDT",
                        SqlDbType.VarChar,
                        10
                    ).Value = maCTDT;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Tìm CTĐT
        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_TimCTDT",
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
            ChuongTrinhDaoTaoDTO ct)
        {
            cmd.Parameters.Add(
                "@MaCTDT",
                SqlDbType.VarChar,
                10
            ).Value = ct.MaCTDT;

            cmd.Parameters.Add(
                "@TenCTDT",
                SqlDbType.NVarChar,
                50
            ).Value = ct.TenCTDT;

            cmd.Parameters.Add(
                "@TrinhDo",
                SqlDbType.NVarChar,
                20
            ).Value = ct.TrinhDo;

            cmd.Parameters.Add(
                "@ThoiGianDaoTao",
                SqlDbType.Int
            ).Value = ct.ThoiGianDaoTao;

            cmd.Parameters.Add(
                "@TongTinChi",
                SqlDbType.Int
            ).Value = ct.TongTinChi;

            cmd.Parameters.Add(
                "@NamBatDau",
                SqlDbType.Int
            ).Value = ct.NamBatDau;

            cmd.Parameters.Add(
                "@NamKetThuc",
                SqlDbType.Int
            ).Value = ct.NamKetThuc;

            cmd.Parameters.Add(
                "@MaNganh",
                SqlDbType.VarChar,
                10
            ).Value = ct.MaNganh;
        }
    }
}
