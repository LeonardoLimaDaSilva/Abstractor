using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class ApplicationEventLifetimeScopeDecoratorTests
    {
        [Theory, AutoMoqData]
        public void Handle_HasCurrentLifetimeScope_ShouldHandleEvent(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IApplicationEventHandler<IApplicationEvent>> eventHandler,
            IApplicationEvent applicationEvent,
            ApplicationEventLifetimeScopeDecorator<IApplicationEvent> decorator)
        {
            // Act

            decorator.Handle(applicationEvent);

            // Assert

            eventHandler.Verify(h => h.Handle(applicationEvent), Times.Once);

            container.Verify(c => c.BeginLifetimeScope(), Times.Never);
        }

        [Theory, AutoMoqData]
        public void Handle_HasNoCurrentLifetimeScope_ShouldBeginNewLifetimeScopeBeforeHandleEvent(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IApplicationEventHandler<IApplicationEvent>> eventHandler,
            IApplicationEvent applicationEvent,
            ApplicationEventLifetimeScopeDecorator<IApplicationEvent> decorator)
        {
            // Arrange

            var callOrder = 0;

            container.Setup(c => c.GetCurrentLifetimeScope()).Returns(null);
            container.Setup(c => c.BeginLifetimeScope()).Callback(() => callOrder++.Should().Be(0));
            eventHandler.Setup(c => c.Handle(applicationEvent)).Callback(() => callOrder++.Should().Be(1));

            // Act

            decorator.Handle(applicationEvent);

            // Assert

            container.Verify(c => c.BeginLifetimeScope(), Times.Once);

            eventHandler.Verify(h => h.Handle(applicationEvent), Times.Once);
        }
    }
}