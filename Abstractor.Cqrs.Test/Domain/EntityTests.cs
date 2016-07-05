using System;
using Abstractor.Cqrs.Infrastructure.Domain;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain
{
    public class EntityTests
    {
        [Theory]
        [InlineData("c480311c-3838-4449-bf99-e37e32a4b376", "c480311c-3838-4449-bf99-e37e32a4b376")]
        internal void Id_Constructor_ShouldBe(string id, string expected)
        {
            new ConcreteEntity(Guid.Parse(id)).Id.Should().Be(Guid.Parse(expected));
        }

        [Theory]
        [InlineData("c480311c-3838-4449-bf99-e37e32a4b376", 532182307)]
        [InlineData("00000000-0000-0000-0000-000000000000", 0)]
        internal void GetHashCode_ShouldBe(string id, int expected)
        {
            new ConcreteEntity(Guid.Parse(id)).GetHashCode().Should().Be(expected);
        }

        [Fact]
        internal void Equals_SameIds_ShouldBeTrue()
        {
            var entity1 = new ConcreteEntity(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));
            var entity2 = new ConcreteEntity(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));

            entity1.Equals(entity2).Should().Be.True();
            entity2.Equals(entity1).Should().Be.True();
        }

        [Fact]
        internal void Equals_DifferentEntitiesWithSameIds_ShouldBeTrue()
        {
            var entity1 = new ConcreteEntity(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));
            var entity2 = new ConcreteEntity2(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));
            var entity3 = new ConcreteEntity3(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));

            entity1.Equals(entity2).Should().Be.False();
            entity2.Equals(entity1).Should().Be.False();

            entity1.Equals(entity3).Should().Be.False();
            entity3.Equals(entity1).Should().Be.False();
        }

        [Fact]
        internal void Equals_InheritedEntityWithSameIds_ShouldBeTrue()
        {
            var entity1 = new ConcreteEntity2(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));
            var entity2 = new ConcreteEntity3(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));

            entity1.Equals(entity2).Should().Be.True();
            entity2.Equals(entity1).Should().Be.True();
        }

        [Fact]
        internal void Equals_DifferentIds_ShouldBeFalse()
        {
            var entity1 = new ConcreteEntity(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));
            var entity2 = new ConcreteEntity(Guid.Parse("6b13f222-fa43-4903-ad92-1aab76f6065a"));

            entity1.Equals(entity2).Should().Be.False();
            entity2.Equals(entity1).Should().Be.False();
        }

        [Fact]
        internal void Equals_OtherIsNull_ShouldBeFalse()
        {
            var entity1 = new ConcreteEntity(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));

            entity1.Equals(null).Should().Be.False();
        }

        [Fact]
        internal void Equals_OtherIsTransient_ShouldBeFalse()
        {
            var entity1 = new ConcreteEntity(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));
            var entity2 = new ConcreteEntity(Guid.Empty);

            entity1.Equals(entity2).Should().Be.False();
            entity2.Equals(entity1).Should().Be.False();
        }

        [Fact]
        internal void Equals_BothAreTransient_ShouldBeFalse()
        {
            var entity1 = new ConcreteEntity(Guid.Empty);
            var entity2 = new ConcreteEntity(Guid.Empty);

            entity1.Equals(entity2).Should().Be.False();
        }

        [Fact]
        internal void Equals_SameInstance_ShouldBeTrue()
        {
            var entity1 = new ConcreteEntity(Guid.Parse("c480311c-3838-4449-bf99-e37e32a4b376"));
            var entity2 = entity1;

            entity1.Equals(entity2).Should().Be.True();
            entity2.Equals(entity1).Should().Be.True();
        }

        [Fact]
        internal void Equals_SameInstanceTransient_ShouldBeTrue()
        {
            var entity1 = new ConcreteEntity(Guid.Empty);
            var entity2 = entity1;

            entity1.Equals(entity2).Should().Be.True();
            entity2.Equals(entity1).Should().Be.True();
        }

        private class ConcreteEntity : Entity<Guid>
        {
            public ConcreteEntity(Guid id)
                : base(id)
            {
            }
        }

        private class ConcreteEntity2 : Entity<Guid>
        {
            public ConcreteEntity2(Guid id)
                : base(id)
            {
            }
        }

        private class ConcreteEntity3 : ConcreteEntity2
        {
            public ConcreteEntity3(Guid id)
                : base(id)
            {
            }
        }
    }
}