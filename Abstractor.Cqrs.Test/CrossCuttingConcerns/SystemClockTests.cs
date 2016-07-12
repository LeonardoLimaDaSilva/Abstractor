using System;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.CrossCuttingConcerns
{
    public class SystemClockTests
    {
        [Fact]
        public void Now_ShouldBeGreaterThanMinValue()
        {
            // Act

            var now = new SystemClock().Now();

            // Assert

            now.Should().Be.GreaterThan(DateTime.MinValue);
        }
    }
}