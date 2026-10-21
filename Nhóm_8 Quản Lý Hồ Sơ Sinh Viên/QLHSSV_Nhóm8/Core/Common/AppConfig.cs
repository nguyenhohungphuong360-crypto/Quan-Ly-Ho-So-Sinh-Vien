using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace QLHSSV_Nhóm8.Core.Common
{
    /// <summary>
    /// Quản lý cấu hình toàn cục từ appsettings.json
    /// </summary>
    public static class AppConfig
    {
        private static readonly IConfiguration Configuration;

        static AppConfig()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public static string GetConnectionString(string name = "DefaultConnection")
        {
            var connStr = Configuration.GetConnectionString(name);
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                return connStr;
            }

            // Fallback an toàn nếu chưa có cấu hình ngoài
            return @"Server=(localdb)\MSSQLLocalDB;Database=QLHSSV;Integrated Security=True;TrustServerCertificate=True;";
        }

        public static int WorkFactor => 
            Configuration.GetValue<int>("Security:WorkFactor", 11);

        public static bool EnableLegacyPlainTextMigration =>
            Configuration.GetValue<bool>("Security:EnableLegacyPlainTextMigration", true);

        public static int DeadlockMaxRetries =>
            Configuration.GetValue<int>("DeadlockPolicy:MaxRetryAttempts", 3);

        public static int DeadlockBaseDelayMs =>
            Configuration.GetValue<int>("DeadlockPolicy:BaseDelayMilliseconds", 200);
    }
}

