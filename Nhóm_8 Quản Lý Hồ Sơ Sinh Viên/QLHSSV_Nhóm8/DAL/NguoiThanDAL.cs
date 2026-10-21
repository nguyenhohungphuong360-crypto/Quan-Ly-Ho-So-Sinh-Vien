using System;
using System.Data;
using Microsoft.Data.SqlClient;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL
{
    public class NguoiThanDAL : QLHSSV_Nhóm8.DAL.Interfaces.INguoiThanDAL
    {
        // Lấy danh sách từ View
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT *
                    FROM dbo.vw_NguoiThanDayDu
                    ORDER BY MaNT";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // Kiểm tra mã người thân tồn tại
        public bool KiemTraTonTai(string maNT)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_KiemTraNguoiThanTonTai", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaNT",
                        SqlDbType.VarChar,
                        10
                    ).Value = maNT;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Thêm bằng Stored Procedure
        public bool Insert(NguoiThanDTO nt)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_ThemNT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, nt);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Sửa bằng Stored Procedure
        public bool Update(NguoiThanDTO nt)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_SuaNT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    GanThamSo(cmd, nt);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Xóa bằng Stored Procedure
        public bool Delete(string maNT)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_XoaNT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaNT",
                        SqlDbType.VarChar,
                        10
                    ).Value = maNT;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Tìm bằng Stored Procedure
        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_TimNguoiThan", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@TuKhoa",
                        SqlDbType.NVarChar,
                        100
                    ).Value = keyword;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
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
            NguoiThanDTO nt)
        {
            cmd.Parameters.Add("@MaNT", SqlDbType.VarChar, 10).Value = nt.MaNT;
            cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 50).Value = nt.HoTen;
            cmd.Parameters.Add("@MoiQuanHe", SqlDbType.NVarChar, 30).Value = nt.MoiQuanHe;
            cmd.Parameters.Add("@NgheNghiep", SqlDbType.NVarChar, 50).Value = nt.NgheNghiep;
            cmd.Parameters.Add("@SDT", SqlDbType.VarChar, 20).Value = nt.SDT;
            cmd.Parameters.Add("@NgaySinh", SqlDbType.Date).Value = nt.NgaySinh;
            cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 100).Value = nt.DiaChi;
            cmd.Parameters.Add("@DanToc", SqlDbType.NVarChar, 20).Value = nt.DanToc;
            cmd.Parameters.Add("@GioiTinh", SqlDbType.NVarChar, 10).Value = nt.GioiTinh;
            cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 10).Value = nt.MaSV;
        }
    }
}
