using System;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class CommandPostActionDecoratorTests
    {
        [Theory]
        [AutoMoqData]
        public void Handle_HandlerThrowsException_ShouldNotCallActAndShouldReset(
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            [Frozen] Mock<ICommandPostAction> commandPostAction,
            ICommand command,
            CommandPostActionDecorator<ICommand> decorator)
        {
            // Arrange

            commandHandler.Setup(c => c.Handle(It.IsAny<ICommand>())).Throws<Exception>();

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            commandPostAction.Verify(a => a.Act(), Times.Never);

            commandPostAction.Verify(a => a.Reset(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Handle_Success_ShouldCallActAfterCommandHandleAndThenReset(
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            [Frozen] Mock<ICommandPostAction> commandPostAction,
            ICommand command,
            CommandPostActionDecorator<ICommand> decorator)
        {
            // Arrange

            var callOrder = 0;

            commandHandler.Setup(c => c.Handle(command)).Callback(() => callOrder++.Should().Be(0));
            commandPostAction.Setup(a => a.Act()).Callback(() => callOrder++.Should().Be(1));
            commandPostAction.Setup(a => a.Reset()).Callback(() => callOrder++.Should().Be(2));

            // Act

            decorator.Handle(command);

            // Assert

            commandHandler.Verify(h => h.Handle(command), Times.Once);

            commandPostAction.Verify(a => a.Act(), Times.Once);

            commandPostAction.Verify(a => a.Reset(), Times.Once);
        }
    }
}