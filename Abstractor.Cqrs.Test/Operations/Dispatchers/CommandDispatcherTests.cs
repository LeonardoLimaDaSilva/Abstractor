using System;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Dispatchers
{
    public class CommandDispatcherTests
    {
        [Theory, AutoMoqData]
        internal void Dispatch_NullCommand_ThrowsArgumentNullException(CommandDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => dispatcher.Dispatch(null));
        }

        [Theory, AutoMoqData]
        internal void Dispatch_BuildGenericCommandHandlerAndGetFromContainer_ShouldHandleCommand(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<FakeCommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher
            )
        {
            // Arrange

            var genericTypeName = typeof (ICommandHandler<FakeCommand>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(commandHandler.Object);

            // Act

            dispatcher.Dispatch(command);

            // Assert

            commandHandler.Verify(t => t.Handle(command), Times.Once);
        }

        [Theory, AutoMoqData]
        internal async void DispatchAsync_NullCommand_ThrowsArgumentNullException(CommandDispatcher dispatcher)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => dispatcher.DispatchAsync(null));
        }

        [Theory, AutoMoqData]
        internal async void DispatchAsync_BuildGenericCommandHandlerAndGetFromContainer_ShouldHandleCommandOnANewThread(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<FakeCommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher
            )
        {
            // Arrange

            var genericTypeName = typeof (ICommandHandler<FakeCommand>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(commandHandler.Object);

            // Act

            await dispatcher.DispatchAsync(command);

            // Assert

            commandHandler.Verify(t => t.Handle(command), Times.Once);
        }

        public class FakeCommand : ICommand
        {
        }
    }
}