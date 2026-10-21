using System.Data;
using QLHSSV_Nhóm8.Core.Common;
using QLHSSV_Nhóm8.Core.Security;
using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.DTO;
using Serilog;

namespace QLHSSV_Nhóm8.BUS
{
    public class TaiKhoanBUS
    {
        private readonly ITaiKhoanDAL _dal;
        private readonly IPasswordHasher _passwordHasher;

        // Constructor mặc định tương thích ngược cho WinForms
        public TaiKhoanBUS() : this(new TaiKhoanDAL(), new PasswordHasher())
        {
        }

        // Constructor phục vụ Dependency Injection & Unit Testing
        public TaiKhoanBUS(ITaiKhoanDAL dal, IPasswordHasher passwordHasher)
        {
            _dal = dal;
            _passwordHasher = passwordHasher;
        }

        // Lấy danh sách tài khoản
        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        // Thêm tài khoản kèm băm mật khẩu chuẩn BCrypt
        public bool Them(TaiKhoanDTO tk)
        {
            if (!DuLieuHopLe(tk))
                return false;

            if (_dal.KiemTraTonTai(tk.TenDangNhap))
                return false;

            // Băm mật khẩu an toàn trước khi lưu vào database
            tk.MatKhau = _passwordHasher.HashPassword(tk.MatKhau);

            bool success = _dal.Insert(tk);
            if (success)
            {
                Log.Information("Tài khoản {Username} đã được tạo thành công với vai trò {Role}.", tk.TenDangNhap, tk.VaiTro);
            }
            return success;
        }

        // Sửa tài khoản
        public bool Sua(TaiKhoanDTO tk)
        {
            if (!DuLieuHopLe(tk))
                return false;

            if (!_dal.KiemTraTonTai(tk.TenDangNhap))
                return false;

            // Nếu mật khẩu được sửa và chưa phải là chuỗi hash BCrypt thì băm mới
            if (!tk.MatKhau.StartsWith("$2a$") && !tk.MatKhau.StartsWith("$2b$") && !tk.MatKhau.StartsWith("$2y$"))
            {
                tk.MatKhau = _passwordHasher.HashPassword(tk.MatKhau);
            }

            return _dal.Update(tk);
        }

        // Xóa tài khoản
        public bool Xoa(string tenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
                return false;

            tenDangNhap = tenDangNhap.Trim();

            // Không xóa admin chính
            if (tenDangNhap.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                Log.Warning("Cảnh báo: Phát hiện thao tác cố gắng xóa tài khoản quản trị hệ thống '{Admin}'.", tenDangNhap);
                return false;
            }

            return _dal.Delete(tenDangNhap);
        }

        // Tìm kiếm tài khoản
        public DataTable TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return _dal.GetAll();

            return _dal.Search(keyword.Trim());
        }

        // Đăng nhập an toàn (Hỗ trợ BCrypt + Tự động di trú mật khẩu plaintext cũ)
        public TaiKhoanDTO? DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
                return null;

            tenDangNhap = tenDangNhap.Trim();

            // 1. Lấy thông tin tài khoản theo tên đăng nhập
            var tk = _dal.GetByUsername(tenDangNhap);
            if (tk == null)
            {
                // Fallback: Thử gọi Stored Procedure cũ nếu có
                var legacyResult = _dal.DangNhap(tenDangNhap, matKhau);
                if (legacyResult != null)
                {
                    Log.Information("Đăng nhập thành công tài khoản {Username} qua legacy procedure.", tenDangNhap);
                }
                return legacyResult;
            }

            // Kiểm tra trạng thái kích hoạt của tài khoản
            if (!tk.TrangThai)
            {
                Log.Warning("Đăng nhập thất bại: Tài khoản {Username} đang bị khóa hoặc vô hiệu hóa.", tenDangNhap);
                return null;
            }

            // 2. Xác thực mật khẩu và kiểm tra nhu cầu nâng cấp (Migration)
            bool isValid = _passwordHasher.VerifyAndCheckMigration(matKhau, tk.MatKhau, out bool needsMigration);

            if (!isValid)
            {
                Log.Warning("Đăng nhập thất bại: Sai mật khẩu cho tài khoản {Username}.", tenDangNhap);
                return null;
            }

            // 3. Tự động di trú sang mật khẩu băm BCrypt nếu tài khoản còn dùng mật khẩu cũ
            if (needsMigration)
            {
                try
                {
                    string newHash = _passwordHasher.HashPassword(matKhau);
                    _dal.UpdatePassword(tenDangNhap, newHash);
                    tk.MatKhau = newHash;
                    Log.Information("Bảo mật: Đã tự động nâng cấp mật khẩu tài khoản {Username} sang chuẩn BCrypt thành công.", tenDangNhap);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Lỗi khi tự động di trú mật khẩu tài khoản {Username}.", tenDangNhap);
                }
            }

            Log.Information("Tài khoản {Username} đăng nhập thành công vào hệ thống.", tenDangNhap);
            return tk;
        }

        // Kiểm tra dữ liệu hợp lệ
        public bool DuLieuHopLe(TaiKhoanDTO tk)
        {
            if (tk == null)
                return false;

            return !string.IsNullOrWhiteSpace(tk.TenDangNhap) &&
                   !string.IsNullOrWhiteSpace(tk.MatKhau) &&
                   !string.IsNullOrWhiteSpace(tk.VaiTro);
        }
    }
}
