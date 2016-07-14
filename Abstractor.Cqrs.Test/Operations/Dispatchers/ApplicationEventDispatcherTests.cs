using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Dispatchers
{
    public class ApplicationEventDispatcherTests
    {
        public class FakeApplicationEvent : IApplicationEvent
        {
        }

        [Theory, AutoMoqData]
        public void Dispatch_NullEvent_ThrowsArgumentNullException(ApplicationEventDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => dispatcher.Dispatch(null));
        }
        
        [Theory, AutoMoqData]
        public void Dispatch_SyncContext_BuildGenericType_ExecutesAllHandlersPassingTheEvent(
            [Frozen] Mock<IContainer> container,
            Mock<IApplicationEventHandler<FakeApplicationEvent>> eventHandler1,
            Mock<IApplicationEventHandler<FakeApplicationEvent>> eventHandler2,
            FakeApplicationEvent applicationEvent,
            ApplicationEventDispatcher dispatcher)
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var genericTypeName = typeof (IApplicationEventHandler<FakeApplicationEvent>).FullName;

            var eventHandlers = new List<IApplicationEventHandler<FakeApplicationEvent>>
            {
                eventHandler1.Object,
                eventHandler2.Object
            };

            container.Setup(c => c.GetAllInstances(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(eventHandlers);

            Task.Factory.StartNew(() =>
            {
                // Act

                dispatcher.Dispatch(applicationEvent);
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            eventHandler1.Verify(t => t.Handle(applicationEvent), Times.Once);

            eventHandler2.Verify(t => t.Handle(applicationEvent), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Dispatch_SyncContext_NoHandlersRegistered_DoNothing(
            [Frozen] Mock<IContainer> container,
            FakeApplicationEvent applicationEvent,
            ApplicationEventDispatcher dispatcher)
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var genericTypeName = typeof (IApplicationEventHandler<FakeApplicationEvent>).FullName;

            container.Setup(c => c.GetAllInstances(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(new List<IApplicationEventHandler<FakeApplicationEvent>>());

            Task.Factory.StartNew(() =>
            {
                // Act

                dispatcher.Dispatch(applicationEvent);
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);
        }

        [Theory, AutoMoqData]
        public void Dispatch_EventHandler1ThrowsException_ExceptionShouldBeSupressentAndEventHandler2BeExecuted(
            [Frozen] Mock<IContainer> container,
            Mock<IApplicationEventHandler<FakeApplicationEvent>> eventHandler1,
            Mock<IApplicationEventHandler<FakeApplicationEvent>> eventHandler2,
            FakeApplicationEvent applicationEvent,
            ApplicationEventDispatcher dispatcher)
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var genericTypeName = typeof (IApplicationEventHandler<FakeApplicationEvent>).FullName;

            var eventHandlers = new List<IApplicationEventHandler<FakeApplicationEvent>>
            {
                eventHandler1.Object,
                eventHandler2.Object
            };

            container.Setup(c => c.GetAllInstances(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(eventHandlers);

            eventHandler1.Setup(h => h.Handle(It.IsAny<FakeApplicationEvent>())).Throws<Exception>();

            Task.Factory.StartNew(() =>
            {
                // Act

                dispatcher.Dispatch(applicationEvent);
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            eventHandler1.Verify(t => t.Handle(applicationEvent), Times.Once);

            eventHandler2.Verify(t => t.Handle(applicationEvent), Times.Once);
        }
    }
}