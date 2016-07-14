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

        /// <summary>
        ///     Dispatch inside a sync context.
        ///     Build generic event handler and get all registrations for this type from container.
        ///     Should execute all handlers passing the event listener.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="eventHandler1"></param>
        /// <param name="eventHandler2"></param>
        /// <param name="eventListener"></param>
        /// <param name="dispatcher"></param>
        [Theory, AutoMoqData]
        public void Dispatch_SyncContext_BuildGenericType_ExecutesAllHandlersPassingTheEventListener(
            [Frozen] Mock<IContainer> container,
            Mock<IApplicationEventHandler<FakeApplicationEvent>> eventHandler1,
            Mock<IApplicationEventHandler<FakeApplicationEvent>> eventHandler2,
            FakeApplicationEvent eventListener,
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

                dispatcher.Dispatch(eventListener);
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            eventHandler1.Verify(t => t.Handle(eventListener), Times.Once);

            eventHandler2.Verify(t => t.Handle(eventListener), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Dispatch_SyncContext_NoHandlersRegistered_DoNothing(
            [Frozen] Mock<IContainer> container,
            FakeApplicationEvent eventListener,
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

                dispatcher.Dispatch(eventListener);
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
            FakeApplicationEvent eventListener,
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

                dispatcher.Dispatch(eventListener);
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            eventHandler1.Verify(t => t.Handle(eventListener), Times.Once);

            eventHandler2.Verify(t => t.Handle(eventListener), Times.Once);
        }
    }
}