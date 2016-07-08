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
        public class FakeEventListener : IEventListener
        {
        }

        [Theory, AutoMoqData]
        public void Dispatch_NullEvent_ThrowsArgumentNullException(EventDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => dispatcher.Dispatch(null));
        }

        [Theory, AutoMoqData]
        public void Dispatch_BuildGenericEventTriggerAndGetFromContainer_ShouldExecuteTriggerPassingEvent(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IEventTrigger<FakeEventListener>> eventTrigger,
            FakeEventListener eventListener,
            EventDispatcher dispatcher
            )
        {
            // Arrange

            var genericTypeName = typeof (IEventTrigger<FakeEventListener>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(eventTrigger.Object);

            // Act

            dispatcher.Dispatch(eventListener);

            // Assert

            eventTrigger.Verify(t => t.Trigger(eventListener), Times.Once);
        }
    }
}