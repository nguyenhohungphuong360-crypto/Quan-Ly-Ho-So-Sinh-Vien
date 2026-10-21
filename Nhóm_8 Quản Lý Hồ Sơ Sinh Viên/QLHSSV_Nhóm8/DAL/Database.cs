using System;
using Microsoft.Data.SqlClient;
using QLHSSV_Nhóm8.Core.Common;

namespace QLHSSV_Nhóm8.DAL
{
    public class Database
    {
        public static string ConnectionString => AppConfig.GetConnectionString();

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
