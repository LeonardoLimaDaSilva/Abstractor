using System;
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
            public bool ThrowsException { get; set; }

            public bool HasInnerException { get; set; }

            public FakeResult Handle(FakeQuery query)
            {
                if (!ThrowsException) return new FakeResult();

                if (!HasInnerException) throw new Exception("FakeQueryHandlerException.");

                throw new Exception("FakeQueryHandlerException.", new Exception("FakeQueryHandlerInnerException."));
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

            var queryHandler = new FakeQueryHandler();

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            // Act

            decorator.Handle(query).Should().Not.Be.Null();

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Never);
            stopwatch.Verify(s => s.Stop(), Times.Never);

            logger.Verify(l => l.Log(It.IsAny<string>()), Times.Never);
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

            var queryHandler = new FakeQueryHandler();

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            attributeFinder.Setup(f => f.Decorates(query.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(query).Should().Not.Be.Null();

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:00."), Times.Once);
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

            var queryHandler = new FakeQueryHandler();

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
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

            decorator.Handle(query).Should().Not.Be.Null();

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(
                l => l.Log("Could not serialize the parameters: Serialization exception."),
                Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:00."), Times.Once);
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

            var queryHandler = new FakeQueryHandler {ThrowsException = true};

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            attributeFinder.Setup(f => f.Decorates(query.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            var exception = Assert.Throws<Exception>(() => decorator.Handle(query));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeQueryHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:00."), Times.Once);

            exception.Message.Should().Be("FakeQueryHandlerException.");
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

            var queryHandler = new FakeQueryHandler
            {
                ThrowsException = true,
                HasInnerException = true
            };

            var decorator = new QueryLoggerDecorator<FakeQuery, FakeResult>(
                () => queryHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            attributeFinder.Setup(f => f.Decorates(query.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(query)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            var exception = Assert.Throws<Exception>(() => decorator.Handle(query));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing query \"FakeQuery\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeQueryHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Inner exception caught: FakeQueryHandlerInnerException."), Times.Once);
            logger.Verify(l => l.Log("Query \"FakeQuery\" executed in 00:00:00."), Times.Once);

            exception.Message.Should().Be("FakeQueryHandlerException.");
            exception.InnerException.Message.Should().Be("FakeQueryHandlerInnerException.");
        }
    }
}