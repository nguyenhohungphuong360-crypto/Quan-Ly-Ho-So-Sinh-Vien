using System;

namespace QLHSSV_Nhóm8.Core.Security
{
    public interface IPasswordHasher
    {
        string HashPassword(string plainPassword);
        bool VerifyPassword(string plainPassword, string hashedPassword);
        bool VerifyAndCheckMigration(string inputPassword, string storedPassword, out bool needsMigration);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private readonly int _workFactor;
        private readonly bool _enableLegacyMigration;

        public PasswordHasher(int workFactor = 11, bool enableLegacyMigration = true)
        {
            _workFactor = workFactor;
            _enableLegacyMigration = enableLegacyMigration;
        }

        public string HashPassword(string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword))
                throw new ArgumentException("Mật khẩu không được để trống.", nameof(plainPassword));

            return BCrypt.Net.BCrypt.HashPassword(plainPassword, _workFactor);
        }

        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyAndCheckMigration(string inputPassword, string storedPassword, out bool needsMigration)
        {
            needsMigration = false;
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedPassword))
                return false;

            // Kiểm tra xem storedPassword có phải là BCrypt hash hợp lệ hay không ($2a$, $2b$, $2y$)
            bool isBcryptHash = storedPassword.StartsWith("$2a$") || 
                                storedPassword.StartsWith("$2b$") || 
                                storedPassword.StartsWith("$2y$");

            if (isBcryptHash)
            {
                return VerifyPassword(inputPassword, storedPassword);
            }

            // Nếu không phải BCrypt, kiểm tra cơ chế Legacy Plaintext Migration
            if (_enableLegacyMigration && string.Equals(inputPassword, storedPassword, StringComparison.Ordinal))
            {
                needsMigration = true;
                return true;
            }

            return false;
        }
    }
}

