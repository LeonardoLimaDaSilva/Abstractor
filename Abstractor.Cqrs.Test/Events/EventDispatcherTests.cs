using System;
using Abstractor.Cqrs.Infrastructure.Events;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Abstractor.Cqrs.Test.Events
{
    public class EventDispatcherTests
    {
        [Theory, AutoMoqData]
        public void Dispatch_NullEvent_ThrowsArgumentNullException(EventDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => dispatcher.Dispatch(null));
        }

        [Theory, AutoMoqData]
        public void Dispatch_BuildGenericEventTriggerAndGetFromContainer_ShouldExecuteTriggerPassingEvent(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IEventTrigger<FakeEvent>> eventTrigger,
            FakeEvent @event,
            EventDispatcher dispatcher
            )
        {
            // Arrange

            var genericTypeName = typeof(IEventTrigger<FakeEvent>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                .Returns(eventTrigger.Object);

            // Act

            dispatcher.Dispatch(@event);

            // Assert

            eventTrigger.Verify(t => t.Trigger(@event), Times.Once);
        }

        public class FakeEvent : IEvent
        {
        }
    }
}