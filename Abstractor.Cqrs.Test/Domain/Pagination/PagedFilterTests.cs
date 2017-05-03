using Abstractor.Cqrs.Infrastructure.Domain.Pagination;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain.Pagination
{
    public class PagedFilterTests
    {
        public class FakeFilter : PagedFilter
        {
        }

        [Theory]
        [InlineData(null, 1)]
        [InlineData(-1, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public void Page_ShouldNotBeLesserThan1(int page, int expected)
        {
            // Arrange and act

            var filter = new FakeFilter
            {
                Page = page
            };

            // Assert

            filter.Page.Should().Be(expected);
        }

        [Theory]
        [InlineData(0, null, 0)]
        [InlineData(1, null, 0)]
        [InlineData(1, 5, 0)]
        [InlineData(5, 1, 4)]
        [InlineData(5, 3, 12)]
        [InlineData(null, 1, 0)]
        [InlineData(-1, 3, 0)]
        public void Skip_ShouldBeExpected(int? page, int? take, int expected)
        {
            // Arrange and act

            var filter = new FakeFilter
            {
                Page = page,
                Take = take
            };

            // Assert

            filter.Skip.Should().Be(expected);
        }
    }
}