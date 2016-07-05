using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.Events;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Abstractor.Cqrs.Test.Events
{
    public class TriggerRegisteredEventsDecoratorTests
    {
        [Theory, AutoMoqData]
        internal void Trigger_NoHandlersRegistered_ShouldNotTrigger(
            [Frozen] Mock<IEventTrigger<IEvent>> eventTrigger,
            IEvent @event,
            TriggerRegisteredEventsDecorator<IEvent> decorator)
        {
            // Act

            decorator.Trigger(@event);

            // Assert

            eventTrigger.Verify(t => t.Trigger(It.IsAny<IEvent>()), Times.Never);
        }

        [Theory, AutoMoqData]
        internal void Trigger_HasCurrentLifetimeScope_ShouldTrigger(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IEventHandler<IEvent>> fakeEventHandler,
            [Frozen] Mock<IEventTrigger<IEvent>> eventTrigger,
            IEvent @event,
            TriggerRegisteredEventsDecorator<IEvent> decorator)
        {
            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(() =>
            {
                // Arrange

                var registrations = new List<InstanceProducerAdapter>
                {
                    new InstanceProducerAdapter(fakeEventHandler.Object),
                    new InstanceProducerAdapter(fakeEventHandler.Object)
                };

                container.Setup(c => c.GetCurrentRegistrations()).Returns(registrations);

                // Act

                decorator.Trigger(@event);
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            container.Verify(c => c.BeginLifetimeScope(), Times.Never);

            eventTrigger.Verify(t => t.Trigger(It.IsAny<IEvent>()), Times.Once);
        }

        [Theory, AutoMoqData]
        internal void Trigger_HasNoCurrentLifetimeScope_ShouldBeginNewLifetimeScopeAndTrigger(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IEventHandler<IEvent>> fakeEventHandler,
            [Frozen] Mock<IEventTrigger<IEvent>> eventTrigger,
            IEvent @event,
            TriggerRegisteredEventsDecorator<IEvent> decorator)
        {
            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(() =>
            {
                // Arrange

                var registrations = new List<InstanceProducerAdapter>
                {
                    new InstanceProducerAdapter(fakeEventHandler.Object),
                    new InstanceProducerAdapter(fakeEventHandler.Object)
                };

                container.Setup(c => c.GetCurrentRegistrations()).Returns(registrations);

                container.Setup(c => c.GetCurrentLifetimeScope()).Returns(null);

                // Act

                decorator.Trigger(@event);
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            container.Verify(c => c.BeginLifetimeScope(), Times.Once);

            eventTrigger.Verify(t => t.Trigger(It.IsAny<IEvent>()), Times.Once);
        }
    }
}