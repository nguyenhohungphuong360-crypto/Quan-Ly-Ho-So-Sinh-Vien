using FluentAssertions;
using QLHSSV_Nhóm8.Core.Common;
using Xunit;

namespace QLHSSV.Tests.Common
{
    public class ResultPatternTests
    {
        [Fact]
        public void Result_Success_ShouldHaveIsSuccessTrueAndEmptyErrorMessage()
        {
            // Act
            var res = Result.Success();

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.IsFailure.Should().BeFalse();
            res.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void Result_Failure_ShouldHaveIsSuccessFalseAndGivenErrorMessage()
        {
            // Act
            var res = Result.Failure("Đã xảy ra lỗi kết nối cơ sở dữ liệu.");

            // Assert
            res.IsSuccess.Should().BeFalse();
            res.IsFailure.Should().BeTrue();
            res.ErrorMessage.Should().Be("Đã xảy ra lỗi kết nối cơ sở dữ liệu.");
        }

        [Fact]
        public void ResultGeneric_Success_ShouldContainCorrectData()
        {
            // Act
            var res = Result<string>.Success("DataPayload");

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Data.Should().Be("DataPayload");
            res.ErrorMessage.Should().BeEmpty();
        }
    }
}

