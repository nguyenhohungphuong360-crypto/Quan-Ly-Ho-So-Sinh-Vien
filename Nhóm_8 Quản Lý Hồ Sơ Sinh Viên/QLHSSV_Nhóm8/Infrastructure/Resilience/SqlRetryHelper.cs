using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Serilog;
using QLHSSV_Nhóm8.Core.Common;

namespace QLHSSV_Nhóm8.Infrastructure.Resilience
{
    /// <summary>
    /// Chính sách tự động thử lại khi gặp Deadlock (Sql Error 1205) với Exponential Backoff & Jitter
    /// </summary>
    public static class SqlRetryHelper
    {
        private static readonly Random RandomJitter = new();

        public static T ExecuteWithRetry<T>(
            Func<T> operation, 
            string operationName = "DatabaseOperation",
            int? maxRetries = null,
            int? baseDelayMs = null)
        {
            int retries = maxRetries ?? AppConfig.DeadlockMaxRetries;
            int delayMs = baseDelayMs ?? AppConfig.DeadlockBaseDelayMs;
            int attempt = 0;

            while (true)
            {
                attempt++;
                try
                {
                    return operation();
                }
                catch (SqlException ex) when (IsTransientOrDeadlock(ex))
                {
                    if (attempt > retries)
                    {
                        Log.Error(ex, "[{Operation}] Đã thử lại {Attempt}/{MaxRetries} lần nhưng vẫn gặp lỗi Deadlock/Transient. Hủy bỏ tác vụ.",
                            operationName, attempt - 1, retries);
                        throw;
                    }

                    // Tính thời gian chờ theo Exponential Backoff + Jitter
                    int backoffMs = delayMs * (int)Math.Pow(2, attempt - 1);
                    int jitter = RandomJitter.Next(10, 100);
                    int totalWait = backoffMs + jitter;

                    Log.Warning(ex, "[{Operation}] Phát hiện Deadlock (Mã lỗi {SqlNumber}). Đang thử lại lần {Attempt}/{MaxRetries} sau {WaitMs}ms...",
                        operationName, ex.Number, attempt, retries, totalWait);

                    Thread.Sleep(totalWait);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[{Operation}] Lỗi không phục hồi được trong quá trình thao tác Database.", operationName);
                    throw;
                }
            }
        }

        public static void ExecuteWithRetry(
            Action operation, 
            string operationName = "DatabaseOperation",
            int? maxRetries = null,
            int? baseDelayMs = null)
        {
            ExecuteWithRetry<object?>(() =>
            {
                operation();
                return null;
            }, operationName, maxRetries, baseDelayMs);
        }

        public static bool IsTransientOrDeadlock(SqlException ex)
        {
            // 1205: Deadlock victim
            // 1222: Lock request time-out period exceeded
            // -2: Timeout expired
            return ex.Number == 1205 || ex.Number == 1222 || ex.Number == -2;
        }
    }
}

