using System;
using System.Threading;
using System.Windows.Forms;
using QLHSSV_Nhóm8.Core.DI;
using QLHSSV_Nhóm8.GUI;
using QLHSSV_Nhóm8.Infrastructure.Logging;
using Serilog;

namespace QLHSSV_Nhóm8
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // 1. Khởi tạo Structured Logging (Serilog)
            AppLogger.Initialize();

            try
            {
                // 2. Thiết lập cơ chế bắt lỗi toàn cục Unhandled Exception Handler
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                // 3. Khởi tạo cấu hình ứng dụng WinForms & Dependency Injection
                ApplicationConfiguration.Initialize();
                var serviceProvider = ServiceConfigurator.ConfigureServices();

                Log.Information("Ứng dụng QLHSSV khởi động thành công.");

                // 4. Resolve Form từ DI Container
                var loginForm = ServiceConfigurator.GetService<FLogin>();
                Application.Run(loginForm);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Lỗi nghiêm trọng không xử lý được khiến ứng dụng phải dừng lại.");
                MessageBox.Show(
                    $"Đã xảy ra sự cố nghiêm trọng:\n{ex.Message}\n\nVui lòng kiểm tra file nhật ký (logs/qlhssv-*.log) để biết chi tiết.",
                    "Lỗi hệ thống",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                AppLogger.CloseAndFlush();
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Lỗi chưa được xử lý tại UI Thread.");
            MessageBox.Show(
                $"Lỗi giao diện: {e.Exception.Message}",
                "Cảnh báo hệ thống",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Lỗi Domain Unhandled Exception (IsTerminating: {IsTerminating}).", e.IsTerminating);
            }
        }
    }
}
