using System;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class CommandEventDispatcherDecoratorTests
    {
        public interface ICommandThatListensToEvents : ICommand, IEventListener
        {
        }

        [Theory, AutoMoqData]
        public void Handle_Success_ShouldDispatchEventAfterCommandHandled(
            [Frozen] Mock<ICommandHandler<ICommandThatListensToEvents>> commandHandler,
            [Frozen] Mock<IEventDispatcher> eventDispatcher,
            ICommandThatListensToEvents command,
            CommandEventDispatcherDecorator<ICommandThatListensToEvents> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            commandHandler.Setup(d => d.Handle(command)).Callback(() => callOrder++.Should().Be(0));
            eventDispatcher.Setup(d => d.Dispatch(command)).Callback(() => callOrder++.Should().Be(1));

            // Act

            decorator.Handle(command);
        }

        [Theory, AutoMoqData]
        public void Handle_Exception_ShouldNotDispatchEvent(
            [Frozen] Mock<ICommandHandler<ICommandThatListensToEvents>> commandHandler,
            [Frozen] Mock<IEventDispatcher> eventDispatcher,
            ICommandThatListensToEvents command,
            CommandEventDispatcherDecorator<ICommandThatListensToEvents> decorator)
        {
            // Arrange

            commandHandler.Setup(d => d.Handle(It.IsAny<ICommandThatListensToEvents>())).Throws<Exception>();

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            eventDispatcher.Verify(d => d.Dispatch(It.IsAny<ICommandThatListensToEvents>()), Times.Never);
        }

        [Theory, AutoMoqData]
        public void Handle_CommandException_ShouldNotDispatchEvent_ShouldDispatchExceptionAndRethrow(
            [Frozen] Mock<ICommandHandler<ICommandThatListensToEvents>> commandHandler,
            [Frozen] Mock<IEventDispatcher> eventDispatcher,
            ICommandThatListensToEvents command,
            CommandEventDispatcherDecorator<ICommandThatListensToEvents> decorator)
        {
            // Arrange

            commandHandler.Setup(d => d.Handle(It.IsAny<ICommandThatListensToEvents>())).Throws<CommandException>();

            // Act

            Assert.Throws<CommandException>(() => decorator.Handle(command));

            // Assert

            eventDispatcher.Verify(d => d.Dispatch(It.IsAny<CommandException>()), Times.Once);
        }
    }
}