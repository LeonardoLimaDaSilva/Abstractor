using System;
using Abstractor.Cqrs.Infrastructure.CompositionRoot;
using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
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

        public class FakeCommandHandler : CommandHandler<FakeCommand>
        {
            public bool Executed { get; private set; }

            public bool HasInnerException { get; set; }

            public bool ThrowsException { get; set; }

            public override void Handle(FakeCommand command)
            {
                Executed = true;

                if (!ThrowsException) return;

                if (!HasInnerException) throw new Exception("FakeCommandHandlerException.");

                throw new Exception("FakeCommandHandlerException.", new Exception("FakeCommandHandlerInnerException."));
            }
        }

        [Theory]
        [AutoMoqData]
        public void Handle_CommandHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler {ThrowsException = true};
            var logger = new FakeLogger();

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(command.GetType(), typeof(LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            var exception = Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.VerifyMessages(
                "Executing command \"FakeCommand\" with the parameters:",
                "Serialized parameters",
                "Exception caught: FakeCommandHandlerException.",
                "Command \"FakeCommand\" executed in 00:00:00.");

            commandHandler.Executed.Should().Be.True();

            exception.Message.Should().Be("FakeCommandHandlerException.");
        }

        [Theory]
        [AutoMoqData]
        public void Handle_CommandHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
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

            var logger = new FakeLogger();

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(command.GetType(), typeof(LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            var exception = Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.VerifyMessages(
                "Executing command \"FakeCommand\" with the parameters:",
                "Serialized parameters",
                "Exception caught: FakeCommandHandlerException.",
                "Inner exception caught: FakeCommandHandlerInnerException.",
                "Command \"FakeCommand\" executed in 00:00:00.");

            commandHandler.Executed.Should().Be.True();

            exception.Message.Should().Be("FakeCommandHandlerException.");
            exception.InnerException?.Message.Should().Be("FakeCommandHandlerInnerException.");
        }

        [Theory]
        [AutoMoqData]
        public void Handle_Success_ShouldLogMessagesAndCallMethods(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler();
            var logger = new FakeLogger();

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(command.GetType(), typeof(LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(command);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.VerifyMessages(
                "Executing command \"FakeCommand\" with the parameters:",
                "Serialized parameters",
                "Command \"FakeCommand\" executed in 00:00:00.");

            commandHandler.Executed.Should().Be.True();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_ThrowsOnSerialize_ShouldLogException(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler();
            var logger = new FakeLogger();

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(command.GetType(), typeof(LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            loggerSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
                            .Throws(new Exception("Serialization exception."));

            // Act

            decorator.Handle(command);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.VerifyMessages(
                "Executing command \"FakeCommand\" with the parameters:",
                "Could not serialize the parameters: Serialization exception.",
                "Command \"FakeCommand\" executed in 00:00:00.");

            commandHandler.Executed.Should().Be.True();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_WithoutLogAttribute_ShouldNotLog(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler();
            var logger = new FakeLogger();

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(command);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Never);
            stopwatch.Verify(s => s.Stop(), Times.Never);

            logger.ShouldNeverBeCalled();

            commandHandler.Executed.Should().Be.True();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_WithoutLogAttributeAndGloballyEnabled_ShouldLog(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeCommand command)
        {
            // Arrange

            var commandHandler = new FakeCommandHandler();
            var logger = new FakeLogger();

            var decorator = new CommandLoggerDecorator<FakeCommand>(
                () => commandHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings
                {
                    EnableLogging = true
                });

            loggerSerializer.Setup(s => s.Serialize(command)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(command);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.ShouldBeCalled();

            commandHandler.Executed.Should().Be.True();
        }
    }
}