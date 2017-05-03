using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.Domain.Pagination;
using Abstractor.Cqrs.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain.Pagination
{
    public class PagedResultTests
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_ShouldMapProperties(List<object> list, int totalCount)
        {
            // Arrange and act

            var pagedResult = new PagedResult<object>(list, totalCount);

            // Assert

            pagedResult.Result.Should().Have.SameSequenceAs(list);
            pagedResult.TotalCount.Should().Be(totalCount);
        }
    }
}