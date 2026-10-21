using System;
using FluentAssertions;
using Moq;
using QLHSSV.DTO;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.DTO;
using Xunit;

namespace QLHSSV.Tests.Business
{
    public class SinhVienValidationTests
    {
        private readonly Mock<ISinhVienDAL> _mockDal;
        private readonly SinhVienBUS _bus;

        public SinhVienValidationTests()
        {
            _mockDal = new Mock<ISinhVienDAL>();
            _bus = new SinhVienBUS(_mockDal.Object);
        }

        private SinhVienDTO TaoSinhVienHopLe()
        {
            return new SinhVienDTO
            {
                MaSV = "SV999",
                HoTen = "Nguyễn Văn Chuyên Nghiệp",
                CCCD = "079201012345",
                NgaySinh = new DateTime(2003, 5, 20),
                GioiTinh = "Nam",
                SDT = "0987654321",
                Email = "nguyenvanpro@university.edu.vn",
                DiaChi = "TP. Hồ Chí Minh",
                DanToc = "Kinh",
                TrangThaiHocTap = "Đang học",
                MaLop = "CNTT01"
            };
        }

        [Fact]
        public void KiemTraHopLe_ValidStudent_ShouldReturnTrue()
        {
            // Arrange
            var sv = TaoSinhVienHopLe();

            // Act
            bool isValid = _bus.KiemTraHopLe(sv, out string error);

            // Assert
            isValid.Should().BeTrue();
            error.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", "Mã sinh viên không được để trống.")]
        [InlineData("   ", "Mã sinh viên không được để trống.")]
        public void KiemTraHopLe_EmptyMaSV_ShouldReturnFalseWithExpectedError(string maSV, string expectedError)
        {
            // Arrange
            var sv = TaoSinhVienHopLe();
            sv.MaSV = maSV;

            // Act
            bool isValid = _bus.KiemTraHopLe(sv, out string error);

            // Assert
            isValid.Should().BeFalse();
            error.Should().Contain(expectedError);
        }

        [Fact]
        public void KiemTraHopLe_UnderageStudent_ShouldFailValidation()
        {
            // Arrange (Sinh viên dưới 15 tuổi)
            var sv = TaoSinhVienHopLe();
            sv.NgaySinh = DateTime.Today.AddYears(-10);

            // Act
            bool isValid = _bus.KiemTraHopLe(sv, out string error);

            // Assert
            isValid.Should().BeFalse();
            error.Should().Contain("Sinh viên phải từ 15 đến 80 tuổi");
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@domain.com")]
        public void KiemTraHopLe_InvalidEmail_ShouldFailValidation(string invalidEmail)
        {
            // Arrange
            var sv = TaoSinhVienHopLe();
            sv.Email = invalidEmail;

            // Act
            bool isValid = _bus.KiemTraHopLe(sv, out string error);

            // Assert
            isValid.Should().BeFalse();
            error.Should().Contain("Email không hợp lệ");
        }

        [Theory]
        [InlineData("12345")]       // Quá ngắn
        [InlineData("123456789012345")] // Quá dài
        [InlineData("079abc123456")] // Chứa ký tự chữ
        public void KiemTraHopLe_InvalidCCCD_ShouldFailValidation(string invalidCccd)
        {
            // Arrange
            var sv = TaoSinhVienHopLe();
            sv.CCCD = invalidCccd;

            // Act
            bool isValid = _bus.KiemTraHopLe(sv, out string error);

            // Assert
            isValid.Should().BeFalse();
            error.Should().Contain("CCCD/CMND");
        }

        [Fact]
        public void ThemResult_WhenMaSVAlreadyExists_ShouldReturnFailureWithoutCallingInsert()
        {
            // Arrange
            var sv = TaoSinhVienHopLe();
            _mockDal.Setup(d => d.KiemTraTonTai(sv.MaSV)).Returns(true);

            // Act
            var result = _bus.ThemResult(sv);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("đã tồn tại");
            _mockDal.Verify(d => d.Insert(It.IsAny<SinhVienDTO>()), Times.Never);
        }

        [Fact]
        public void ThemResult_WhenDataIsValidAndNotExists_ShouldCallInsertAndReturnSuccess()
        {
            // Arrange
            var sv = TaoSinhVienHopLe();
            _mockDal.Setup(d => d.KiemTraTonTai(sv.MaSV)).Returns(false);
            _mockDal.Setup(d => d.Insert(sv)).Returns(true);

            // Act
            var result = _bus.ThemResult(sv);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _mockDal.Verify(d => d.Insert(sv), Times.Once);
        }
    }
}

