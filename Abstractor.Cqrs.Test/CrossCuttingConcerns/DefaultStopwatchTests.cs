using System;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.CrossCuttingConcerns
{
    public class DefaultStopwatchTests
    {
        [Fact]
        public void GetElapsed_TimespanShouldBeGreaterThanZero()
        {
            // Arrange

            var stopwatch = new DefaultStopwatch();

            stopwatch.Start();
            stopwatch.Stop();

            // Act

            var elapsed = stopwatch.GetElapsed();

            // Assert

            elapsed.Should().Not.Be(TimeSpan.Zero);
        }
    }
}