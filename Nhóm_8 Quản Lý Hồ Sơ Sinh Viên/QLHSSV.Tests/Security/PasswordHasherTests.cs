using FluentAssertions;
using QLHSSV_Nhóm8.Core.Security;
using Xunit;

namespace QLHSSV.Tests.Security
{
    public class PasswordHasherTests
    {
        private readonly IPasswordHasher _hasher;

        public PasswordHasherTests()
        {
            _hasher = new PasswordHasher(workFactor: 10, enableLegacyMigration: true);
        }

        [Fact]
        public void HashPassword_ShouldReturnValidBcryptString()
        {
            // Arrange
            string plainText = "Admin@123456";

            // Act
            string hash = _hasher.HashPassword(plainText);

            // Assert
            hash.Should().NotBeNullOrWhiteSpace();
            hash.Should().StartWith("$2"); // BCrypt standard prefix ($2a$, $2b$)
            hash.Length.Should().Be(60);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            string password = "SecretPassword123!";
            string hash = _hasher.HashPassword(password);

            // Act
            bool isValid = _hasher.VerifyPassword(password, hash);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
        {
            // Arrange
            string password = "SecretPassword123!";
            string hash = _hasher.HashPassword(password);

            // Act
            bool isValid = _hasher.VerifyPassword("WrongPassword", hash);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void VerifyAndCheckMigration_WithLegacyPlainText_ShouldVerifyAndFlagMigration()
        {
            // Arrange (Hệ thống cũ lưu mật khẩu thô dạng plaintext)
            string legacyPlainTextInDb = "123456";
            string userInput = "123456";

            // Act
            bool isValid = _hasher.VerifyAndCheckMigration(userInput, legacyPlainTextInDb, out bool needsMigration);

            // Assert
            isValid.Should().BeTrue();
            needsMigration.Should().BeTrue();
        }

        [Fact]
        public void VerifyAndCheckMigration_WithModernBcryptHash_ShouldVerifyWithoutMigration()
        {
            // Arrange
            string password = "MyModernSecurePassword";
            string bcryptHash = _hasher.HashPassword(password);

            // Act
            bool isValid = _hasher.VerifyAndCheckMigration(password, bcryptHash, out bool needsMigration);

            // Assert
            isValid.Should().BeTrue();
            needsMigration.Should().BeFalse();
        }
    }
}

