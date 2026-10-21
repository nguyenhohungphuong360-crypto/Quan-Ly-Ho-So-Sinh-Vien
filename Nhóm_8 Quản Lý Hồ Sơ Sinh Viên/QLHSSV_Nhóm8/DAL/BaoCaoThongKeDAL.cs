using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace QLHSSV_Nhóm8.DAL
{
    public class BaoCaoThongKeDAL : QLHSSV_Nhóm8.DAL.Interfaces.IBaoCaoThongKeDAL
    {
        // Đếm sinh viên trong khoa bằng Function
        public int DemSinhVienKhoa(string maKhoa)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql =
                    "SELECT dbo.fn_DemSSVKhoa(@MaKhoa)";

                using (SqlCommand cmd =
                       new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@MaKhoa",
                        SqlDbType.VarChar,
                        10
                    ).Value = maKhoa;

                    return Convert.ToInt32(
                        cmd.ExecuteScalar());
                }
            }
        }

        // Đếm số ngành trong khoa bằng Function
        public int DemSoNganhKhoa(string maKhoa)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql =
                    "SELECT dbo.fn_DemSoNganhKhoa(@MaKhoa)";

                using (SqlCommand cmd =
                       new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@MaKhoa",
                        SqlDbType.VarChar,
                        10
                    ).Value = maKhoa;

                    return Convert.ToInt32(
                        cmd.ExecuteScalar());
                }
            }
        }

        // Đếm sinh viên theo CTĐT bằng Function
        public int DemSinhVienCTDT(string maCTDT)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql =
                    "SELECT dbo.fn_DemSVCTDT(@MaCTDT)";

                using (SqlCommand cmd =
                       new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@MaCTDT",
                        SqlDbType.VarChar,
                        10
                    ).Value = maCTDT;

                    return Convert.ToInt32(
                        cmd.ExecuteScalar());
                }
            }
        }
    }
}
