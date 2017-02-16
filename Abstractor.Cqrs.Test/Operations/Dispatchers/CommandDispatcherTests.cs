using System;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
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
        public class FakeCommand : ICommand
        {
        }

        [Theory]
        [AutoMoqData]
        public void Dispatch_BuildGenericCommandHandlerAndGetFromContainer_ShouldHandleCommand(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<FakeCommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher)
        {
            // Arrange

            var genericTypeName = typeof(ICommandHandler<FakeCommand>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(commandHandler.Object);

            // Act

            dispatcher.Dispatch(command);

            // Assert

            commandHandler.Verify(t => t.Handle(command), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Dispatch_CommandThrowsActivationException_ShouldThrowCommandHandlerNotFoundException(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher)
        {
            // Arrange

            commandHandler.Setup(h => h.Handle(It.IsAny<FakeCommand>())).Throws<Exception>();

            container.Setup(c => c.GetInstance(It.IsAny<Type>())).Throws<Exception>();

            container.Setup(c => c.IsActivationException(It.IsAny<Exception>())).Returns(true);

            // Act and assert

            Assert.Throws<CommandHandlerNotFoundException>(() => dispatcher.Dispatch(command));
        }

        [Theory]
        [AutoMoqData]
        public void Dispatch_CommandThrowsCommandException_ShouldHandleExceptionAndRethrow(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher)
        {
            // Arrange

            var commandException = new CommandException("Exception");

            commandHandler.Setup(h => h.Handle(It.IsAny<FakeCommand>())).Throws(commandException);

            var genericTypeName = typeof(ICommandHandler<FakeCommand>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(commandHandler.Object);

            var commandExceptionGenericTypeName = typeof(ICommandHandler<CommandException>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == commandExceptionGenericTypeName)))
                     .Returns(commandHandler.Object);

            // Act

            Assert.Throws<CommandException>(() => dispatcher.Dispatch(command));

            // Assert

            commandHandler.Verify(t => t.Handle(command), Times.Once);

            commandHandler.Verify(h => h.Handle(commandException), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Dispatch_CommandThrowsGenericException_ShouldJustRethrow(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher)
        {
            // Arrange

            commandHandler.Setup(h => h.Handle(It.IsAny<FakeCommand>())).Throws<Exception>();

            var genericTypeName = typeof(ICommandHandler<FakeCommand>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(commandHandler.Object);

            // Act

            Assert.Throws<Exception>(() => dispatcher.Dispatch(command));

            // Assert

            commandHandler.Verify(t => t.Handle(It.IsAny<ICommand>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Dispatch_NullCommand_ThrowsArgumentNullException(CommandDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => dispatcher.Dispatch(null));
        }

        [Theory]
        [AutoMoqData]
        public async void DispatchAsync_BuildGenericCommandHandlerAndGetFromContainer_ShouldHandleCommandOnANewThread(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<FakeCommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher)
        {
            // Arrange

            var genericTypeName = typeof(ICommandHandler<FakeCommand>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(commandHandler.Object);

            // Act

            await dispatcher.DispatchAsync(command);

            // Assert

            commandHandler.Verify(t => t.Handle(command), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async void DispatchAsync_CommandThrowsActivationException_ShouldThrowCommandHandlerNotFoundException(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher)
        {
            // Arrange

            commandHandler.Setup(h => h.Handle(It.IsAny<FakeCommand>())).Throws<Exception>();

            container.Setup(c => c.GetInstance(It.IsAny<Type>())).Throws<Exception>();

            container.Setup(c => c.IsActivationException(It.IsAny<Exception>())).Returns(true);

            // Act and assert

            await Assert.ThrowsAsync<CommandHandlerNotFoundException>(() => dispatcher.DispatchAsync(command));
        }

        [Theory]
        [AutoMoqData]
        public async void DispatchAsync_CommandThrowsCommandException_ShouldHandleExceptionAndRethrow(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher)
        {
            // Arrange

            var commandException = new CommandException();

            commandHandler.Setup(h => h.Handle(It.IsAny<FakeCommand>())).Throws(commandException);

            var genericTypeName = typeof(ICommandHandler<FakeCommand>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(commandHandler.Object);

            var commandExceptionGenericTypeName = typeof(ICommandHandler<CommandException>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == commandExceptionGenericTypeName)))
                     .Returns(commandHandler.Object);

            // Act

            await Assert.ThrowsAsync<CommandException>(() => dispatcher.DispatchAsync(command));

            // Assert

            commandHandler.Verify(t => t.Handle(command), Times.Once);

            commandHandler.Verify(h => h.Handle(commandException), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async void DispatchAsync_CommandThrowsGenericException_ShouldJustRethrow(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            FakeCommand command,
            CommandDispatcher dispatcher)
        {
            // Arrange

            commandHandler.Setup(h => h.Handle(It.IsAny<FakeCommand>())).Throws<Exception>();

            var genericTypeName = typeof(ICommandHandler<FakeCommand>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                     .Returns(commandHandler.Object);

            // Act

            await Assert.ThrowsAsync<Exception>(() => dispatcher.DispatchAsync(command));

            // Assert

            commandHandler.Verify(t => t.Handle(It.IsAny<ICommand>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async void DispatchAsync_NullCommand_ThrowsArgumentNullException(CommandDispatcher dispatcher)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => dispatcher.DispatchAsync(null));
        }
    }
}