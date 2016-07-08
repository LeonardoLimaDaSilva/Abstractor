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
    public class MultipleDispatchEventTriggerTests
    {
        [Theory, AutoMoqData]
        public void Trigger_MultipleEventHandlersRegisteredInContainer_ShouldHandleAll(
            [Frozen] Mock<IContainer> container,
            Mock<IEventHandler<IEventListener>> fakeEventHandler1,
            Mock<IEventHandler<IEventListener>> fakeEventHandler2,
            [Frozen] Mock<object> fakeService,
            IEventListener eventListener,
            MultipleDispatchEventTrigger<IEventListener> trigger)
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var registrations = new List<InstanceProducerAdapter>
            {
                new InstanceProducerAdapter(fakeEventHandler1.Object),
                new InstanceProducerAdapter(fakeEventHandler2.Object),
                new InstanceProducerAdapter(fakeService.Object)
            };

            container.Setup(c => c.GetCurrentRegistrations()).Returns(registrations);

            Task.Factory.StartNew(
                () =>
                {
                    // Act

                    trigger.Trigger(eventListener);
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            fakeEventHandler1.Verify(h => h.Handle(eventListener), Times.Once);
            fakeEventHandler2.Verify(h => h.Handle(eventListener), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Trigger_NoEventHandlersRegistered_ShouldDoNothing(
            [Frozen] Mock<IContainer> container,
            IEventListener eventListener,
            MultipleDispatchEventTrigger<IEventListener> trigger)
        {
            // Arrange

            container.Setup(c => c.GetCurrentRegistrations()).Returns(new List<InstanceProducerAdapter>());

            // Act

            trigger.Trigger(eventListener);
        }
    }
}