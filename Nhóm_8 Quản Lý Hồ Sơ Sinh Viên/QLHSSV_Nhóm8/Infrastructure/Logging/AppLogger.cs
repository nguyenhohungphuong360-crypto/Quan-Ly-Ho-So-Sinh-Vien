using System;
using System.IO;
using Serilog;
using Serilog.Events;

namespace QLHSSV_Nhóm8.Infrastructure.Logging
{
    public static class AppLogger
    {
        private static bool _initialized = false;
        private static readonly object _lock = new();

        public static void Initialize()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var logPath = Path.Combine(logDirectory, "qlhssv-.log");

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.File(
                        logPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30,
                        fileSizeLimitBytes: 10 * 1024 * 1024,
                        rollOnFileSizeLimit: true,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                _initialized = true;
                Log.Information("=== QLHSSV System Initialized ===");
            }
        }

        public static void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }
    }
}

