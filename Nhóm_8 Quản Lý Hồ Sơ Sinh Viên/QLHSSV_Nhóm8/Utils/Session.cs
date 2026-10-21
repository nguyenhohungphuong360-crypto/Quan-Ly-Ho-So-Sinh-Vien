namespace QLHSSV_Nhóm8.Utils
{
    public static class Session
    {
        public static string TenDangNhap { get; set; } = "";

        public static string VaiTro { get; set; } = "";

        // Xóa phiên đăng nhập
        public static void DangXuat()
        {
            TenDangNhap = "";
            VaiTro = "";
        }
    }
}