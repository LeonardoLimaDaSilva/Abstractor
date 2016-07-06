using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.Events;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Abstractor.Cqrs.Test.Events
{
    public class MultipleDispatchEventTriggerTests
    {
        [Theory, AutoMoqData]
        public void Trigger_MultipleEventHandlersRegisteredInContainer_ShouldHandleAll(
            [Frozen] Mock<IContainer> container,
            Mock<IEventHandler<IEvent>> fakeEventHandler1,
            Mock<IEventHandler<IEvent>> fakeEventHandler2,
            [Frozen] Mock<object> fakeService,
            IEvent @event,
            MultipleDispatchEventTrigger<IEvent> trigger)
        {
            // Arrange

            var registrations = new List<InstanceProducerAdapter>
            {
                new InstanceProducerAdapter(fakeEventHandler1.Object),
                new InstanceProducerAdapter(fakeEventHandler2.Object),
                new InstanceProducerAdapter(fakeService.Object)
            };

            container.Setup(c => c.GetCurrentRegistrations()).Returns(registrations);

            // Act

            trigger.Trigger(@event);

            // Assert

            fakeEventHandler1.Verify(h => h.Handle(@event), Times.Once);
            fakeEventHandler2.Verify(h => h.Handle(@event), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Trigger_NoEventHandlersRegistered_ShouldDoNothing(
            [Frozen] Mock<IContainer> container,
            IEvent @event,
            MultipleDispatchEventTrigger<IEvent> trigger)
        {
            // Arrange

            container.Setup(c => c.GetCurrentRegistrations()).Returns(new List<InstanceProducerAdapter>());

            // Act

            trigger.Trigger(@event);
        }
    }
}