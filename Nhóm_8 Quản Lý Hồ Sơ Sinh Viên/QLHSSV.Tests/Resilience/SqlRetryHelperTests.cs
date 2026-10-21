using System;
using FluentAssertions;
using QLHSSV_Nhóm8.Infrastructure.Resilience;
using Xunit;

namespace QLHSSV.Tests.Resilience
{
    public class SqlRetryHelperTests
    {
        [Fact]
        public void ExecuteWithRetry_WhenActionSucceedsImmediately_ShouldReturnExpectedResult()
        {
            // Arrange
            int callCount = 0;

            // Act
            int result = SqlRetryHelper.ExecuteWithRetry(() =>
            {
                callCount++;
                return 42;
            }, "TestImmediateSuccess");

            // Assert
            result.Should().Be(42);
            callCount.Should().Be(1);
        }

        [Fact]
        public void ExecuteWithRetry_WhenPermanentExceptionOccurs_ShouldThrowImmediately()
        {
            // Arrange
            int callCount = 0;

            // Act
            Action act = () =>
            {
                SqlRetryHelper.ExecuteWithRetry<int>(() =>
                {
                    callCount++;
                    throw new InvalidOperationException("Fatal business error");
                }, "TestPermanentFailure", maxRetries: 3);
            };

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Fatal business error");

            callCount.Should().Be(1); // Không retry với non-SQL exception
        }
    }
}

