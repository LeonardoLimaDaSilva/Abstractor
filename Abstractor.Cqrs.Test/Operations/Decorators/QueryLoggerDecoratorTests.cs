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
    public class QueryLoggerDecoratorTests
    {
        public class FakeQuery : IQuery<FakeResult>
        {
        }

        public class FakeResult
        {
        }

        public class FakeQueryHandler : IQueryHandler<FakeQuery, FakeResult>
        {
            public bool Executed { get; private set; }

            public bool ThrowsException { get; set; }

            public bool HasInnerException { get; set; }

            public FakeResult Handle(FakeQuery query)
            {
                Executed = true;

                if (!ThrowsException) return new FakeResult();

                if (!HasInnerException) throw new Exception("FakeQueryHandlerException.");

                throw new Exception("FakeQueryHandlerException.", new Exception("FakeQueryHandlerInnerException."));
            }
        }

        [Theory, AutoMoqData]
        public void Handle_Success_ShouldLogMessagesAndCallMethods(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryHandler();

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.FromSeconds(1));

            // Act

            decorator.Handle(query).Should().Not.Be.Null();

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:01."), Times.Once);

            queryHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsOnSerialize_ShouldLogException(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryHandler();

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.FromSeconds(1));

            loggerSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
                            .Throws(new Exception("Serialization exception."));

            // Act

            decorator.Handle(query).Should().Not.Be.Null();

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(
                l => l.Log("Could not serialize the parameters: Serialization exception."),
                Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:01."), Times.Once);

            queryHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_QueryHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryHandler {ThrowsException = true};

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.FromSeconds(1));

            // Act

            var exception = Assert.Throws<Exception>(() => decorator.Handle(query));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeQueryHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:01."), Times.Once);

            queryHandler.Executed.Should().Be.True();

            exception.Message.Should().Be("FakeQueryHandlerException.");
        }

        [Theory, AutoMoqData]
        public void Handle_QueryHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryHandler
            {
                ThrowsException = true,
                HasInnerException = true
            };

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.FromSeconds(1));

            // Act

            var exception = Assert.Throws<Exception>(() => decorator.Handle(query));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeQueryHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Inner exception caught: FakeQueryHandlerInnerException."), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:01."), Times.Once);

            queryHandler.Executed.Should().Be.True();

            exception.Message.Should().Be("FakeQueryHandlerException.");
            exception.InnerException.Message.Should().Be("FakeQueryHandlerInnerException.");
        }
    }
}