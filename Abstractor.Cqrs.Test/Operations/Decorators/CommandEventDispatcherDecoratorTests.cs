using System;
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
        [Theory, AutoMoqData]
        public void Handle_Success_ShouldDispatchEventAfterCommandHandled(
            [Frozen] Mock<ICommandHandler<ICommandEvent>> commandHandler,
            [Frozen] Mock<IEventDispatcher> eventDispatcher,
            ICommandEvent commandEvent,
            CommandEventDispatcherDecorator<ICommandEvent> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            commandHandler.Setup(d => d.Handle(commandEvent)).Callback(() => callOrder++.Should().Be(0));
            eventDispatcher.Setup(d => d.Dispatch(commandEvent)).Callback(() => callOrder++.Should().Be(1));

            // Act

            decorator.Handle(commandEvent);
        }

        [Theory, AutoMoqData]
        public void Handle_Exception_ShouldNotDispatchEvent(
            [Frozen] Mock<ICommandHandler<ICommandEvent>> commandHandler,
            [Frozen] Mock<IEventDispatcher> eventDispatcher,
            ICommandEvent commandEvent,
            CommandEventDispatcherDecorator<ICommandEvent> decorator)
        {
            // Arrange

            commandHandler.Setup(d => d.Handle(It.IsAny<ICommandEvent>())).Throws<Exception>();

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(commandEvent));

            // Assert

            eventDispatcher.Verify(d => d.Dispatch(It.IsAny<ICommandEvent>()), Times.Never);
        }
    }
}