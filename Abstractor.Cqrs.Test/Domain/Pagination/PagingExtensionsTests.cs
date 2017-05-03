using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.Domain.Pagination;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain.Pagination
{
    public class PagingExtensionsTests
    {
        public class FakeClass
        {
            public string PropertyName { get; set; }
        }

        [Fact]
        public void OrderByField_LowecasePropertyName_Ascending_ShouldOrderAscending()
        {
            // Arrange

            var list = new List<FakeClass>
            {
                new FakeClass
                {
                    PropertyName = "A"
                },
                new FakeClass
                {
                    PropertyName = "B"
                },
                new FakeClass
                {
                    PropertyName = "C"
                }
            };

            // Act

            var ordered = list.AsQueryable().OrderByField("propertyname", OrderDirection.Ascending).ToList();

            // Assert

            ordered[0].PropertyName.Should().Be("A");
            ordered[1].PropertyName.Should().Be("B");
            ordered[2].PropertyName.Should().Be("C");
        }

        [Fact]
        public void OrderByField_Descending_ShouldOrderDescending()
        {
            // Arrange

            var list = new List<FakeClass>
            {
                new FakeClass
                {
                    PropertyName = "A"
                },
                new FakeClass
                {
                    PropertyName = "B"
                },
                new FakeClass
                {
                    PropertyName = "C"
                }
            };

            // Act

            var ordered = list.AsQueryable().OrderByField("PropertyName", OrderDirection.Descending).ToList();

            // Assert

            ordered[0].PropertyName.Should().Be("C");
            ordered[1].PropertyName.Should().Be("B");
            ordered[2].PropertyName.Should().Be("A");
        }

        [Fact]
        public void OrderByField_InvalidProperty_ThrowsException()
        {
            // Arrange, act and assert

            Assert.Throws<ArgumentException>(() =>
                new List<FakeClass>()
                    .AsQueryable()
                    .OrderByField("InvalidProperty", OrderDirection.Ascending));
        }
    }
}