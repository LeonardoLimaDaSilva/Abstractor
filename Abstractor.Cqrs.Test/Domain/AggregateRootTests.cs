using System;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Events;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain
{
    public class AggregateRootTests
    {
        public class FakeAggregateRoot : AggregateRoot<int>
        {
            public FakeAggregateRoot(int id) : base(id)
            {
            }
        }

        public class FakeEvent : IDomainEvent
        {
        }

        [Fact]
        public void Constructor_Id_ShouldBeSetted()
        {
            // Arrange

            var id = new Random().Next();

            // Act

            var fake = new FakeAggregateRoot(id);

            // Assert
            fake.Id.Should().Be(id);
        }

        [Fact]
        public void Constructor_EmittedEvents_ShouldBeInitialized()
        {
            // Arrange and act

            var fake = new FakeAggregateRoot(1);

            // Assert

            fake.EmittedEvents.Count.Should().Be(0);
        }

        [Fact]
        public void Emit_EmittedEvents_EventsShouldBeStored()
        {
            // Arrange and act

            var fake = new FakeAggregateRoot(1);

            fake.Emit(new FakeEvent());
            fake.Emit(new FakeEvent());

            // Assert

            fake.EmittedEvents.Count.Should().Be(2);

            fake.EmittedEvents.All(e=>e.GetType() == typeof(FakeEvent)).Should().Be.True();
        }
    }
}