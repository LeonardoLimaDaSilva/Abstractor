using System;
using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Dispatchers
{
    public class DomainEventDispatcherTests
    {
        public class FakeDomainEvent : IDomainEvent
        {
        }

        [Theory, AutoMoqData]
        public void Dispatch_NullEvent_ThrowsArgumentNullException(DomainEventDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => dispatcher.Dispatch(null));
        }

        [Theory, AutoMoqData]
        public void Dispatch_BuildGenericType_ExecutesAllHandlersPassingTheEvent(
            [Frozen] Mock<IContainer> container,
            Mock<IDomainEventHandler<FakeDomainEvent>> eventHandler1,
            Mock<IDomainEventHandler<FakeDomainEvent>> eventHandler2,
            FakeDomainEvent domainEvent,
            DomainEventDispatcher dispatcher)
        {
            // Arrange

            var genericTypeName = typeof (IDomainEventHandler<FakeDomainEvent>).FullName;

            var eventHandlers = new List<IDomainEventHandler<FakeDomainEvent>>
            {
                eventHandler1.Object,
                eventHandler2.Object
            };

            container.Setup(c => c.GetAllInstances(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(eventHandlers);

            // Act

            dispatcher.Dispatch(domainEvent);

            // Assert

            eventHandler1.Verify(t => t.Handle(domainEvent), Times.Once);

            eventHandler2.Verify(t => t.Handle(domainEvent), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Dispatch_NoHandlersRegistered_DoNothing(
            [Frozen] Mock<IContainer> container,
            FakeDomainEvent domainEvent,
            DomainEventDispatcher dispatcher)
        {
            // Arrange

            var genericTypeName = typeof (IDomainEventHandler<FakeDomainEvent>).FullName;

            container.Setup(c => c.GetAllInstances(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(new List<IDomainEventHandler<FakeDomainEvent>>());

            // Act

            dispatcher.Dispatch(domainEvent);
        }

        [Theory, AutoMoqData]
        public void Dispatch_EventHandler1ThrowsException_ExecutionStopsAndRethrow(
            [Frozen] Mock<IContainer> container,
            Mock<IDomainEventHandler<FakeDomainEvent>> eventHandler1,
            Mock<IDomainEventHandler<FakeDomainEvent>> eventHandler2,
            FakeDomainEvent domainEvent,
            DomainEventDispatcher dispatcher)
        {
            // Arrange

            var genericTypeName = typeof (IDomainEventHandler<FakeDomainEvent>).FullName;

            var eventHandlers = new List<IDomainEventHandler<FakeDomainEvent>>
            {
                eventHandler1.Object,
                eventHandler2.Object
            };

            container.Setup(c => c.GetAllInstances(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(eventHandlers);

            eventHandler1.Setup(h => h.Handle(It.IsAny<FakeDomainEvent>())).Throws<Exception>();

            // Act

            Assert.Throws<Exception>(() => dispatcher.Dispatch(domainEvent));

            // Assert

            eventHandler1.Verify(t => t.Handle(domainEvent), Times.Once);

            eventHandler2.Verify(t => t.Handle(domainEvent), Times.Never);
        }
    }
}