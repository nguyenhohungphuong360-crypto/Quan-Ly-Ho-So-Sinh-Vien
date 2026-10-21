using QLHSSV.DTO;
using QLHSSV_Nhóm8.DTO;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace QLHSSV_Nhóm8.DAL
{
    public class SinhVienDAL : QLHSSV_Nhóm8.DAL.Interfaces.ISinhVienDAL
    {
        // Lấy danh sách sinh viên từ View
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
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
                    ORDER BY MaSV";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // Thêm sinh viên bằng Stored Procedure
        public bool Insert(SinhVienDTO sv)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_ThemSinhVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    GanThamSoSinhVien(cmd, sv);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Sửa sinh viên bằng Stored Procedure
        public bool Update(SinhVienDTO sv)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_SuaSinhVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    GanThamSoSinhVien(cmd, sv);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        // Xóa sinh viên bằng Stored Procedure
        public bool Delete(string maSV)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_XoaSinhVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaSV",
                        SqlDbType.VarChar,
                        10
                    ).Value = maSV;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Tìm sinh viên bằng Stored Procedure
        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand("dbo.sp_TimSinhVien", conn))
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

        // Gán tham số cho Thêm/Sửa
        private void GanThamSoSinhVien(
            SqlCommand cmd,
            SinhVienDTO sv)
        {
            cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 10).Value = sv.MaSV;
            cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 50).Value = sv.HoTen;
            cmd.Parameters.Add("@CCCD", SqlDbType.VarChar, 20).Value = sv.CCCD;
            cmd.Parameters.Add("@NgaySinh", SqlDbType.Date).Value = sv.NgaySinh;
            cmd.Parameters.Add("@GioiTinh", SqlDbType.NVarChar, 10).Value = sv.GioiTinh;
            cmd.Parameters.Add("@SDT", SqlDbType.VarChar, 20).Value = sv.SDT;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = sv.Email;
            cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 100).Value = sv.DiaChi;
            cmd.Parameters.Add("@DanToc", SqlDbType.NVarChar, 20).Value = sv.DanToc;
            cmd.Parameters.Add("@TrangThaiHocTap", SqlDbType.NVarChar, 50).Value = sv.TrangThaiHocTap;
            cmd.Parameters.Add("@MaLop", SqlDbType.VarChar, 10).Value = sv.MaLop;
        }

        // Kiểm tra mã sinh viên tồn tại
        public bool KiemTraTonTai(string maSV)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_KiemTraSinhVienTonTai",
                           conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaSV",
                        SqlDbType.VarChar,
                        10
                    ).Value = maSV;

                    return Convert.ToBoolean(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Thống kê theo lớp từ View
        public DataTable ThongKeTheoLop()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT *
                    FROM dbo.vw_SLSV_Lop
                    ORDER BY MaLop";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // Thống kê theo giới tính từ View
        public DataTable ThongKeTheoGioiTinh()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT *
                    FROM dbo.vw_SLSV_GioiTinh";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // Đếm sinh viên trong lớp bằng Function
        public int DemSinhVienTheoLop(string maLop)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "SELECT dbo.fn_DemSVLop(@MaLop)",
                           conn))
                {
                    cmd.Parameters.Add(
                        "@MaLop",
                        SqlDbType.VarChar,
                        10
                    ).Value = maLop;

                    return Convert.ToInt32(
                        cmd.ExecuteScalar()
                    );
                }
            }
        }

        // Lấy lịch sử xóa từ View
        public DataTable GetLichSuXoa()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT
                        Id,
                        MaSV,
                        HoTen,
                        CCCD,
                        MaLop,
                        ThoiGianXoa
                    FROM dbo.vw_LichSuXoaSinhVien
                    ORDER BY ThoiGianXoa DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }
        // Chuyển lớp
        public bool ChuyenLop(
            string maSV,
            string maLopMoi)
        {
            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                       new SqlCommand(
                           "dbo.sp_ChuyenLop",
                           conn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@MaSV",
                        SqlDbType.VarChar,
                        10
                    ).Value = maSV;

                    cmd.Parameters.Add(
                        "@MaLopMoi",
                        SqlDbType.VarChar,
                        10
                    ).Value = maLopMoi;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
