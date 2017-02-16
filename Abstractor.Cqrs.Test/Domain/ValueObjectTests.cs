using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.Domain;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain
{
    public class ValueObjectTests
    {
        private class ValueObject1 : ValueObject<ValueObject1>
        {
            public int Property1 { private get; set; }

            public string Property2 { private get; set; }

            protected override IEnumerable<object> GetAttributesToIncludeInEqualityCheck()
            {
                yield return Property1;
                yield return Property2;
            }
        }

        [Fact]
        public void DifferenceOperator_AllValuesAreEqual_ShouldBeFalse()
        {
            var vo1 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            var vo2 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            (vo1 != vo2).Should().Be.False();
        }

        [Theory]
        [InlineData(1, "xxx", 1, "xyz")]
        [InlineData(1, "xxx", 2, "xxx")]
        [InlineData(1, "xxx", 2, "xyz")]
        public void DifferenceOperator_AnyValueIsDifferent_ShouldBeTrue(
            int p1Vo1,
            string p2Vo1,
            int p1Vo2,
            string p2Vo2)
        {
            var vo1 = new ValueObject1
            {
                Property1 = p1Vo1,
                Property2 = p2Vo1
            };

            var vo2 = new ValueObject1
            {
                Property1 = p1Vo2,
                Property2 = p2Vo2
            };

            (vo1 != vo2).Should().Be.True();
        }

        [Fact]
        public void Equals_AllValuesAreEqual_ShouldBeTrue()
        {
            var vo1 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            var vo2 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            vo1.Equals(vo2).Should().Be.True();
        }

        [Theory]
        [InlineData(1, "xxx", 1, "xyz")]
        [InlineData(1, "xxx", 2, "xxx")]
        [InlineData(1, "xxx", 2, "xyz")]
        public void Equals_AnyValueIsDifferent_ShouldBeFalse(int p1Vo1, string p2Vo1, int p1Vo2, string p2Vo2)
        {
            var vo1 = new ValueObject1
            {
                Property1 = p1Vo1,
                Property2 = p2Vo1
            };

            var vo2 = new ValueObject1
            {
                Property1 = p1Vo2,
                Property2 = p2Vo2
            };

            vo1.Equals(vo2).Should().Be.False();
        }

        [Fact]
        public void EqualsOperator_AllValuesAreEqual_ShouldBeTrue()
        {
            var vo1 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            var vo2 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            (vo1 == vo2).Should().Be.True();
        }

        [Theory]
        [InlineData(1, "xxx", 1, "xyz")]
        [InlineData(1, "xxx", 2, "xxx")]
        [InlineData(1, "xxx", 2, "xyz")]
        public void EqualsOperator_AnyValueIsDifferent_ShouldBeFalse(int p1Vo1, string p2Vo1, int p1Vo2, string p2Vo2)
        {
            var vo1 = new ValueObject1
            {
                Property1 = p1Vo1,
                Property2 = p2Vo1
            };

            var vo2 = new ValueObject1
            {
                Property1 = p1Vo2,
                Property2 = p2Vo2
            };

            (vo1 == vo2).Should().Be.False();
        }

        [Fact]
        public void GetHashCode_AllValuesAreEqual_ShouldHaveEqualHashCodes()
        {
            var vo1 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            var vo2 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            vo1.GetHashCode().Equals(vo2.GetHashCode()).Should().Be.True();
        }

        [Fact]
        public void GetHashCode_AnyValueIsDifferent_ShouldHaveDifferentHashCodes()
        {
            var vo1 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xxx"
            };

            var vo2 = new ValueObject1
            {
                Property1 = 1,
                Property2 = "xyz"
            };

            vo1.GetHashCode().Equals(vo2.GetHashCode()).Should().Be.False();
        }
    }
}