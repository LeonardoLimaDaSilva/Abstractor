using System;
using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class CommandLoggerDecoratorTests
    {
        public class FakeCommand : ICommand
        {
        }

        public class FakeCommandHandler : ICommandHandler<FakeCommand>
        {
            public bool Executed { get; private set; }

            public bool ThrowsException { get; set; }

            public bool HasInnerException { get; set; }
            
            public IEnumerable<IDomainEvent> Handle(FakeCommand command)
            {
                Executed = true;

                if (!ThrowsException) yield break;

                if (!HasInnerException) throw new Exception("FakeCommandHandlerException.");

                throw new Exception("FakeCommandHandlerException.", new Exception("FakeCommandHandlerInnerException."));
            }
        }

        [Theory, AutoMoqData]
        public void Handle_Success_ShouldLogMessagesAndCallMethods(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler();

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(command);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing command \"FakeCommand\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Command \"FakeCommand\" executed in 00:00:00."), Times.Once);

            commandHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsOnSerialize_ShouldLogException(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler();

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            loggerSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
                            .Throws(new Exception("Serialization exception."));

            // Act

            decorator.Handle(command);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing command \"FakeCommand\" with the parameters:"), Times.Once);
            logger.Verify(
                l => l.Log("Could not serialize the parameters: Serialization exception."),
                Times.Once);
            logger.Verify(l => l.Log("Command \"FakeCommand\" executed in 00:00:00."), Times.Once);

            commandHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_CommandHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler {ThrowsException = true};

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            var exception = Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing command \"FakeCommand\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeCommandHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Command \"FakeCommand\" executed in 00:00:00."), Times.Once);

            commandHandler.Executed.Should().Be.True();

            exception.Message.Should().Be("FakeCommandHandlerException.");
        }

        [Theory, AutoMoqData]
        public void Handle_CommandHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler
            {
                ThrowsException = true,
                HasInnerException = true
            };

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            var exception = Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing command \"FakeCommand\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeCommandHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Inner exception caught: FakeCommandHandlerInnerException."), Times.Once);
            logger.Verify(l => l.Log("Command \"FakeCommand\" executed in 00:00:00."), Times.Once);

            commandHandler.Executed.Should().Be.True();

            exception.Message.Should().Be("FakeCommandHandlerException.");
            exception.InnerException.Message.Should().Be("FakeCommandHandlerInnerException.");
        }
    }
}