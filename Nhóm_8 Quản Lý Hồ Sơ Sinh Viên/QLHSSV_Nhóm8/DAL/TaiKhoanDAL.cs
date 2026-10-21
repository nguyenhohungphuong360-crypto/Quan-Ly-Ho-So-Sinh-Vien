using System;
using System.Data;
using Microsoft.Data.SqlClient;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.DTO;
using QLHSSV_Nhóm8.Infrastructure.Resilience;

namespace QLHSSV_Nhóm8.DAL
{
    public class TaiKhoanDAL : ITaiKhoanDAL
    {
        // Lấy danh sách tài khoản
        public DataTable GetAll()
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                var dt = new DataTable();
                using (SqlConnection conn = Database.GetConnection())
                {
                    string sql = @"
                        SELECT
                            TenDangNhap,
                            MatKhau,
                            VaiTro,
                            TrangThai
                        FROM dbo.vw_TaiKhoan
                        ORDER BY TenDangNhap";

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        da.Fill(dt);
                    }
                }
                return dt;
            }, nameof(GetAll));
        }

        // Lấy thông tin tài khoản theo tên đăng nhập
        public TaiKhoanDTO? GetByUsername(string tenDangNhap)
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT TOP 1 TenDangNhap, MatKhau, VaiTro, TrangThai 
                        FROM dbo.TaiKhoan 
                        WHERE TenDangNhap = @TenDangNhap";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@TenDangNhap", SqlDbType.VarChar, 50).Value = tenDangNhap;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return null;

                            return new TaiKhoanDTO
                            {
                                TenDangNhap = reader["TenDangNhap"]?.ToString() ?? "",
                                MatKhau = reader["MatKhau"]?.ToString() ?? "",
                                VaiTro = reader["VaiTro"]?.ToString() ?? "",
                                TrangThai = reader["TrangThai"] != DBNull.Value && Convert.ToBoolean(reader["TrangThai"])
                            };
                        }
                    }
                }
            }, nameof(GetByUsername));
        }

        // Kiểm tra tài khoản tồn tại
        public bool KiemTraTonTai(string tenDangNhap)
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_KiemTraTaiKhoanTonTai", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@TenDangNhap", SqlDbType.NVarChar, 50).Value = tenDangNhap;

                        var result = cmd.ExecuteScalar();
                        return result != null && result != DBNull.Value && Convert.ToBoolean(result);
                    }
                }
            }, nameof(KiemTraTonTai));
        }

        // Cập nhật mật khẩu băm mới (phục vụ di trú hoặc đổi mật khẩu)
        public bool UpdatePassword(string tenDangNhap, string newHashedPassword)
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        UPDATE dbo.TaiKhoan 
                        SET MatKhau = @MatKhau 
                        WHERE TenDangNhap = @TenDangNhap";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@TenDangNhap", SqlDbType.VarChar, 50).Value = tenDangNhap;
                        cmd.Parameters.Add("@MatKhau", SqlDbType.VarChar, 100).Value = newHashedPassword;

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }, nameof(UpdatePassword));
        }

        // Thêm tài khoản
        public bool Insert(TaiKhoanDTO tk)
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_ThemTaiKhoan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        GanThamSo(cmd, tk);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }, nameof(Insert));
        }

        // Sửa tài khoản
        public bool Update(TaiKhoanDTO tk)
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_SuaTaiKhoan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        GanThamSo(cmd, tk);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }, nameof(Update));
        }

        // Xóa tài khoản
        public bool Delete(string tenDangNhap)
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_XoaTaiKhoan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@TenDangNhap", SqlDbType.NVarChar, 50).Value = tenDangNhap;

                        var res = cmd.ExecuteScalar();
                        return res != null && res != DBNull.Value && Convert.ToBoolean(res);
                    }
                }
            }, nameof(Delete));
        }

        // Tìm tài khoản
        public DataTable Search(string keyword)
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                var dt = new DataTable();
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_TimTaiKhoan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 100).Value = keyword;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                return dt;
            }, nameof(Search));
        }

        // Đăng nhập tương thích ngược với Stored Procedure cũ
        public TaiKhoanDTO? DangNhap(string tenDangNhap, string matKhau)
        {
            return SqlRetryHelper.ExecuteWithRetry(() =>
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_DangNhap", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@TenDangNhap", SqlDbType.NVarChar, 50).Value = tenDangNhap;
                        cmd.Parameters.Add("@MatKhau", SqlDbType.VarChar, 100).Value = matKhau;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return null;

                            return new TaiKhoanDTO
                            {
                                TenDangNhap = reader["TenDangNhap"]?.ToString() ?? "",
                                MatKhau = reader["MatKhau"]?.ToString() ?? "",
                                VaiTro = reader["VaiTro"]?.ToString() ?? "",
                                TrangThai = reader["TrangThai"] != DBNull.Value && Convert.ToBoolean(reader["TrangThai"])
                            };
                        }
                    }
                }
            }, nameof(DangNhap));
        }

        // Gán tham số Thêm/Sửa
        private void GanThamSo(SqlCommand cmd, TaiKhoanDTO tk)
        {
            cmd.Parameters.Add("@TenDangNhap", SqlDbType.NVarChar, 50).Value = tk.TenDangNhap;
            cmd.Parameters.Add("@MatKhau", SqlDbType.VarChar, 100).Value = tk.MatKhau;
            cmd.Parameters.Add("@VaiTro", SqlDbType.NVarChar, 20).Value = tk.VaiTro;
            cmd.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = tk.TrangThai;
        }
    }
}
