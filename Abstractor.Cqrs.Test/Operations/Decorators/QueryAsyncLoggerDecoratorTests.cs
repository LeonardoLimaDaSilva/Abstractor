using System;
using System.Threading.Tasks;
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
    public class QueryAsyncLoggerDecoratorTests
    {
        public class FakeQuery : IQuery<FakeResult>
        {
        }

        public class FakeResult
        {
        }

        public class FakeQueryAsyncHandler : IQueryAsyncHandler<FakeQuery, FakeResult>
        {
            public bool Executed { get; private set; }

            public bool ThrowsException { get; set; }

            public bool HasInnerException { get; set; }

            public Task<FakeResult> HandleAsync(FakeQuery query)
            {
                Executed = true;

                if (!ThrowsException) return null;

                if (!HasInnerException) throw new Exception("FakeQueryAsyncHandlerException.");

                throw new Exception("FakeQueryAsyncHandlerException.",
                    new Exception("FakeQueryAsyncHandlerInnerException."));
            }
        }

        [Theory, AutoMoqData]
        public void Handle_WithoutLogAttribute_ShouldNotLog(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryAsyncHandler();

            var decorator = new QueryAsyncLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.HandleAsync(query);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Never);
            stopwatch.Verify(s => s.Stop(), Times.Never);

            logger.Verify(l => l.Log(It.IsAny<string>()), Times.Never);

            queryHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_Success_ShouldLogMessagesAndCallMethods(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryAsyncHandler();

            var decorator = new QueryAsyncLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            attributeFinder.Setup(f => f.Decorates(query.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.HandleAsync(query);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:00."), Times.Once);

            queryHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsOnSerialize_ShouldLogException(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryAsyncHandler();

            var decorator = new QueryAsyncLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            attributeFinder.Setup(f => f.Decorates(query.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            loggerSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
                            .Throws(new Exception("Serialization exception."));

            // Act

            decorator.HandleAsync(query);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(
                l => l.Log("Could not serialize the parameters: Serialization exception."),
                Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:00."), Times.Once);

            queryHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_QueryHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryAsyncHandler {ThrowsException = true};

            var decorator = new QueryAsyncLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            attributeFinder.Setup(f => f.Decorates(query.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            var exception = Assert.ThrowsAsync<Exception>(() => decorator.HandleAsync(query)).Result;

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeQueryAsyncHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:00."), Times.Once);

            queryHandler.Executed.Should().Be.True();

            exception.Message.Should().Be("FakeQueryAsyncHandlerException.");
        }

        [Theory, AutoMoqData]
        public void Handle_QueryHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeQuery query)
        {
            // Arrange

            var queryHandler = new FakeQueryAsyncHandler
            {
                ThrowsException = true,
                HasInnerException = true
            };

            var decorator = new QueryAsyncLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            attributeFinder.Setup(f => f.Decorates(query.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            var exception = Assert.ThrowsAsync<Exception>(() => decorator.HandleAsync(query)).Result;

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeQueryAsyncHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Inner exception caught: FakeQueryAsyncHandlerInnerException."), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:00."), Times.Once);

            queryHandler.Executed.Should().Be.True();

            exception.Message.Should().Be("FakeQueryAsyncHandlerException.");
            exception.InnerException.Message.Should().Be("FakeQueryAsyncHandlerInnerException.");
        }
    }
}