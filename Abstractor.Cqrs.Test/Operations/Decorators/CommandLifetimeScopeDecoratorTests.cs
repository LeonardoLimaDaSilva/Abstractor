using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class CommandLifetimeScopeDecoratorTests
    {
        [Theory, AutoMoqData]
        internal void Handle_HasCurrentLifetimeScope_ShouldHandleCommand(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            ICommand command,
            CommandLifetimeScopeDecorator<ICommand> decorator)
        {
            // Act

            decorator.Handle(command);

            // Assert

            commandHandler.Verify(h => h.Handle(command), Times.Once);

            container.Verify(c => c.BeginLifetimeScope(), Times.Never);
        }

        [Theory, AutoMoqData]
        internal void Handle_HasNoCurrentLifetimeScope_ShouldBeginNewLifetimeScopeBeforeHandleCommand(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            ICommand command,
            CommandLifetimeScopeDecorator<ICommand> decorator)
        {
            // Arrange

            var callOrder = 0;

            container.Setup(c => c.GetCurrentLifetimeScope()).Returns(null);
            container.Setup(c => c.BeginLifetimeScope()).Callback(() => (callOrder++).Should().Be(0));
            commandHandler.Setup(c => c.Handle(command)).Callback(() => (callOrder++).Should().Be(1));

            // Act

            decorator.Handle(command);

            // Assert

            container.Verify(c => c.BeginLifetimeScope(), Times.Once);

            commandHandler.Verify(h => h.Handle(command), Times.Once);
        }
    }
}