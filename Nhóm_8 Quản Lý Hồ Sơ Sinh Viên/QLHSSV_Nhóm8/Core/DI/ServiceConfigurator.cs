using System;
using Microsoft.Extensions.DependencyInjection;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.Core.Security;
using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.GUI;

namespace QLHSSV_Nhóm8.Core.DI
{
    public static class ServiceConfigurator
    {
        private static IServiceProvider? _serviceProvider;

        public static IServiceProvider ServiceProvider =>
            _serviceProvider ?? throw new InvalidOperationException("Dependency Injection chưa được khởi tạo. Hãy gọi ConfigureServices() trước.");

        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Core Security
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            // Data Access Layer (DAL)
            services.AddTransient<ITaiKhoanDAL, TaiKhoanDAL>();
            services.AddTransient<ISinhVienDAL, SinhVienDAL>();
            services.AddTransient<IKhoaDAL, KhoaDAL>();
            services.AddTransient<ILopDAL, LopDAL>();
            services.AddTransient<INganhDAL, NganhDAL>();
            services.AddTransient<INguoiThanDAL, NguoiThanDAL>();
            services.AddTransient<IBaoCaoThongKeDAL, BaoCaoThongKeDAL>();
            services.AddTransient<IDemoDeadLockDAL, DemoDeadLockDAL>();
            services.AddTransient<IDemoGiaoTacDAL, DemoGiaoTacDAL>();

            // Business Logic Layer (BUS)
            services.AddTransient<TaiKhoanBUS>();
            services.AddTransient<SinhVienBUS>();
            services.AddTransient<KhoaBUS>();
            services.AddTransient<LopBUS>();
            services.AddTransient<NganhBUS>();
            services.AddTransient<NguoiThanBUS>();
            services.AddTransient<BaoCaoThongKeBUS>();

            // GUI Forms
            services.AddTransient<FLogin>();
            services.AddTransient<FMain>();
            services.AddTransient<FSinhVien>();

            _serviceProvider = services.BuildServiceProvider();
            return _serviceProvider;
        }

        public static T GetService<T>() where T : notnull
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}

