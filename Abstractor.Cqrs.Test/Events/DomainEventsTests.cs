using System;
using Abstractor.Cqrs.Infrastructure.Events;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Events
{
    public class DomainEventsTests
    {
        public class FakeEvent : DomainEvent
        {
        }

        [Fact]
        public void Timestamp_ShouldBeUtcNow()
        {
            // Arrange

            const string format = "yyyy-MM-dd-HH:mm";

            var fakeEvent = new FakeEvent();

            // Act and assert

            fakeEvent.Timestamp.ToString(format).Should().Be(DateTime.UtcNow.ToString(format));
        }
    }
}
