using System;
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
            public string Property { get; } = "Value";
        }

        [Theory, AutoMoqData]
        public void Handle_Success_LoggerShouldBeCalled2TimesBeforeCommandHandlerAnd1TimeAfter(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<ICommandHandler<FakeCommand>> commandHandler,
            FakeCommand command,
            CommandLoggerDecorator<FakeCommand> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing command \"FakeCommand\" with the parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            commandHandler.Setup(h => h.Handle(command)).Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Command \"FakeCommand\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act

            decorator.Handle(command);
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsExceptionOnSerialize_ShouldLogTheException(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<ICommandHandler<FakeCommand>> commandHandler,
            FakeCommand command,
            CommandLoggerDecorator<FakeCommand> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing command \"FakeCommand\" with the parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Throws(new Exception("Serialization exception."));

            logger.Setup(
                l => l.Log(It.Is<string>(s => s == "Could not serialize the parameters: Serialization exception.")))
                .Callback(() => { callOrder++.Should().Be(1); });

            commandHandler.Setup(h => h.Handle(command)).Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Command \"FakeCommand\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act

            decorator.Handle(command);
        }

        [Theory, AutoMoqData]
        public void Handle_CommandHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<ICommandHandler<FakeCommand>> commandHandler,
            FakeCommand command,
            CommandLoggerDecorator<FakeCommand> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing command \"FakeCommand\" with the parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            commandHandler.Setup(h => h.Handle(command)).Throws(new Exception("CommandHandler exception."));

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Exception caught: CommandHandler exception.")))
                .Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Command \"FakeCommand\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act and assert

            var ex = Assert.Throws<Exception>(() => decorator.Handle(command));
            ex.Message.Should().Be("CommandHandler exception.");
        }

        [Theory, AutoMoqData]
        public void Handle_CommandHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<ICommandHandler<FakeCommand>> commandHandler,
            FakeCommand command,
            CommandLoggerDecorator<FakeCommand> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing command \"FakeCommand\" with the parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            commandHandler.Setup(h => h.Handle(command))
                .Throws(new Exception("CommandHandler exception.", new Exception("Inner exception.")));

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Exception caught: CommandHandler exception.")))
                .Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Inner exception caught: Inner exception.")))
                .Callback(() => { callOrder++.Should().Be(3); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Command \"FakeCommand\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(4); });

            // Act and assert

            var ex = Assert.Throws<Exception>(() => decorator.Handle(command));

            ex.Message.Should().Be("CommandHandler exception.");
            ex.InnerException.Message.Should().Be("Inner exception.");
        }
    }
}