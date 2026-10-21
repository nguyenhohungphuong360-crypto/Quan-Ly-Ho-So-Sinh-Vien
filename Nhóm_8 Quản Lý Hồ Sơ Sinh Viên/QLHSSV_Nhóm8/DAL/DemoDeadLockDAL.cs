using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace QLHSSV_Nhóm8.DAL
{
    public class DemoDeadLockDAL : QLHSSV_Nhóm8.DAL.Interfaces.IDemoDeadLockDAL
    {
        private SqlConnection? connT1;
        private SqlConnection? connT2;

        private SqlTransaction? tranT1;
        private SqlTransaction? tranT2;

        // Giữ 2 khóa để mô phỏng trạng thái kẹt
        public async Task<string[]> BatDauMoPhongTheoSlideAsync()
        {
            await RollbackCaHaiAsync();

            try
            {
                connT1 = Database.GetConnection();
                connT2 = Database.GetConnection();

                await connT1.OpenAsync();
                await connT2.OpenAsync();

                tranT1 = connT1.BeginTransaction();
                tranT2 = connT2.BeginTransaction();

                using (SqlCommand cmdT1 =
                       new SqlCommand(
                           @"UPDATE dbo.Lop
                             SET TenLop = N' An ninh mạng'
                             WHERE MaLop = 'L01';",
                           connT1,
                           tranT1))
                {
                    int rows =
                        await cmdT1.ExecuteNonQueryAsync();

                    if (rows == 0)
                        throw new Exception(
                            "Không tìm thấy lớp L01.");
                }

                using (SqlCommand cmdT2 =
                       new SqlCommand(
                           @"UPDATE dbo.SinhVien
                             SET HoTen = N' Đặng Thành An'
                             WHERE MaSV = 'SV01';",
                           connT2,
                           tranT2))
                {
                    int rows =
                        await cmdT2.ExecuteNonQueryAsync();

                    if (rows == 0)
                        throw new Exception(
                            "Không tìm thấy sinh viên SV01.");
                }

                return new string[]
                {
                    "T1 đã giữ khóa L01.",
                    "T2 đã giữ khóa SV01."
                };
            }
            catch
            {
                await RollbackCaHaiAsync();
                throw;
            }
        }

        // Hủy cả 2 transaction theo mô hình slide
        public Task RollbackCaHaiAsync()
        {
            try
            {
                tranT1?.Rollback();
            }
            catch
            {
            }

            try
            {
                tranT2?.Rollback();
            }
            catch
            {
            }

            tranT1?.Dispose();
            tranT2?.Dispose();

            connT1?.Dispose();
            connT2?.Dispose();

            tranT1 = null;
            tranT2 = null;

            connT1 = null;
            connT2 = null;

            return Task.CompletedTask;
        }

        // Khắc phục: T1 và T2 cùng thứ tự L01 -> SV01
        public async Task<string[]> ChayNganDeadlockAsync()
        {
            Task<string> t1 =
                ChaySqlAsync(
                    @"
                    BEGIN TRANSACTION;

                    UPDATE dbo.Lop
                    SET TenLop = N' An ninh mạng'
                    WHERE MaLop = 'L01';

                    WAITFOR DELAY '00:00:10';

                    UPDATE dbo.SinhVien
                    SET TrangThaiHocTap = N' Bảo lưu'
                    WHERE MaSV = 'SV01';

                    COMMIT TRANSACTION;",
                    "T1");

            Task<string> t2 =
                ChaySqlAsync(
                    @"
                    BEGIN TRANSACTION;

                    UPDATE dbo.Lop
                    SET NienKhoa = N'2025-2028'
                    WHERE MaLop = 'L01';

                    WAITFOR DELAY '00:00:10';

                    UPDATE dbo.SinhVien
                    SET HoTen = N' Đặng Thành An'
                    WHERE MaSV = 'SV01';

                    COMMIT TRANSACTION;",
                    "T2");

            return await Task.WhenAll(t1, t2);
        }

        private async Task<string> ChaySqlAsync(
            string sql,
            string ten)
        {
            try
            {
                using (SqlConnection conn =
                       Database.GetConnection())
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd =
                           new SqlCommand(
                               sql,
                               conn))
                    {
                        cmd.CommandTimeout = 45;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return ten +
                    ": COMMIT thành công.";
            }
            catch (SqlException ex)
                when (ex.Number == 1205)
            {
                return ten +
                    ": DEADLOCK VICTIM - ROLLBACK.";
            }
            catch (SqlException ex)
                when (ex.Number == -2)
            {
                return ten +
                    ": TIMEOUT.";
            }
            catch (Exception ex)
            {
                return ten +
                    ": lỗi - " +
                    ex.Message;
            }
        }

        public DataTable GetLopDemo()
        {
            return GetTable(
                @"SELECT
                      MaLop,
                      TenLop,
                      NienKhoa,
                      MaNganh
                  FROM dbo.Lop
                  WHERE MaLop = 'L01';");
        }

        public DataTable GetSinhVienDemo()
        {
            return GetTable(
                @"SELECT
                      MaSV,
                      HoTen,
                      TrangThaiHocTap,
                      MaLop
                  FROM dbo.SinhVien
                  WHERE MaSV = 'SV01';");
        }

        public string KiemTraDuLieuDemo()
        {
            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT
                        CASE WHEN EXISTS
                        (
                            SELECT 1
                            FROM dbo.Lop
                            WHERE MaLop = 'L01'
                        )
                        THEN 1 ELSE 0 END AS CoLop,

                        CASE WHEN EXISTS
                        (
                            SELECT 1
                            FROM dbo.SinhVien
                            WHERE MaSV = 'SV01'
                        )
                        THEN 1 ELSE 0 END AS CoSV;";

                using (SqlCommand cmd =
                       new SqlCommand(
                           sql,
                           conn))
                using (SqlDataReader reader =
                       cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return "Không kiểm tra được dữ liệu demo.";

                    bool coLop =
                        Convert.ToBoolean(
                            reader["CoLop"]);

                    bool coSV =
                        Convert.ToBoolean(
                            reader["CoSV"]);

                    if (!coLop && !coSV)
                        return "Không tìm thấy L01 và SV01.";

                    if (!coLop)
                        return "Không tìm thấy lớp L01.";

                    if (!coSV)
                        return "Không tìm thấy sinh viên SV01.";
                }
            }

            return "";
        }

        private DataTable GetTable(
            string sql)
        {
            DataTable dt =
                new DataTable();

            using (SqlConnection conn =
                   Database.GetConnection())
            using (SqlDataAdapter da =
                   new SqlDataAdapter(
                       sql,
                       conn))
            {
                da.Fill(dt);
            }

            return dt;
        }
    }
}
