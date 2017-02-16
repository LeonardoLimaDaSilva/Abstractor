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
    public class ApplicationEventDispatcherDecoratorTests
    {
        public interface ICommandThatSubscribesToEvents : ICommand, IApplicationEvent
        {
        }

        [Theory]
        [AutoMoqData]
        public void Handle_CommandException_ShouldNotDispatchEvent_ShouldDispatchExceptionAndRethrow(
            [Frozen] Mock<ICommandHandler<ICommandThatSubscribesToEvents>> commandHandler,
            [Frozen] Mock<IApplicationEventDispatcher> eventDispatcher,
            ICommandThatSubscribesToEvents command,
            ApplicationEventDispatcherDecorator<ICommandThatSubscribesToEvents> decorator)
        {
            // Arrange

            commandHandler.Setup(d => d.Handle(It.IsAny<ICommandThatSubscribesToEvents>())).Throws<CommandException>();

            // Act

            Assert.Throws<CommandException>(() => decorator.Handle(command));

            // Assert

            eventDispatcher.Verify(d => d.Dispatch(It.IsAny<CommandException>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Handle_Exception_ShouldNotDispatchEvent(
            [Frozen] Mock<ICommandHandler<ICommandThatSubscribesToEvents>> commandHandler,
            [Frozen] Mock<IApplicationEventDispatcher> eventDispatcher,
            ICommandThatSubscribesToEvents command,
            ApplicationEventDispatcherDecorator<ICommandThatSubscribesToEvents> decorator)
        {
            // Arrange

            commandHandler.Setup(d => d.Handle(It.IsAny<ICommandThatSubscribesToEvents>())).Throws<Exception>();

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            eventDispatcher.Verify(d => d.Dispatch(It.IsAny<ICommandThatSubscribesToEvents>()), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public void Handle_Success_ShouldDispatchEventAfterCommandHandled(
            [Frozen] Mock<ICommandHandler<ICommandThatSubscribesToEvents>> commandHandler,
            [Frozen] Mock<IApplicationEventDispatcher> eventDispatcher,
            ICommandThatSubscribesToEvents command,
            ApplicationEventDispatcherDecorator<ICommandThatSubscribesToEvents> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            commandHandler.Setup(d => d.Handle(command)).Callback(() => callOrder++.Should().Be(0));
            eventDispatcher.Setup(d => d.Dispatch(command)).Callback(() => callOrder++.Should().Be(1));

            // Act

            decorator.Handle(command);
        }
    }
}