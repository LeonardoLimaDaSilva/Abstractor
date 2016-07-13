using System;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class EventLoggerDecoratorTests
    {
        public class FakeEventListener : IEventListener
        {
        }

        public class FakeEventHandler : IEventHandler<FakeEventListener>
        {
            public bool Executed { get; private set; }

            public bool ThrowsException { get; set; }

            public bool HasInnerException { get; set; }

            public void Handle(FakeEventListener eventListener)
            {
                Executed = true;

                if (!ThrowsException) return;

                if (!HasInnerException) throw new Exception("FakeEventHandlerException.");

                throw new Exception("FakeEventHandlerException.", new Exception("FakeEventHandlerInnerException."));
            }
        }

        [Theory, AutoMoqData]
        public void Handle_Success_ShouldLogMessagesAndCallMethods(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeEventListener eventListener)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();

            var decorator = new EventLoggerDecorator<FakeEventListener>(
                () => eventHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(eventListener)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.FromSeconds(1));

            // Act

            decorator.Handle(eventListener);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing event \"FakeEventHandler\" with the listener parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Event \"FakeEventHandler\" executed in 00:00:01."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsOnSerialize_ShouldLogException(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeEventListener eventListener)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();

            var decorator = new EventLoggerDecorator<FakeEventListener>(
                () => eventHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(eventListener)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.FromSeconds(1));

            loggerSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
                            .Throws(new Exception("Serialization exception."));

            // Act

            decorator.Handle(eventListener);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing event \"FakeEventHandler\" with the listener parameters:"), Times.Once);
            logger.Verify(
                l => l.Log("Could not serialize the listener parameters: Serialization exception."),
                Times.Once);
            logger.Verify(l => l.Log("Event \"FakeEventHandler\" executed in 00:00:01."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_EventHandlerThrowsException_ShouldLogTheExceptionAndSupress(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeEventListener eventListener)
        {
            // Arrange

            var eventHandler = new FakeEventHandler {ThrowsException = true};

            var decorator = new EventLoggerDecorator<FakeEventListener>(
                () => eventHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(eventListener)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.FromSeconds(1));

            // Act

            decorator.Handle(eventListener);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing event \"FakeEventHandler\" with the listener parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeEventHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Event \"FakeEventHandler\" executed in 00:00:01."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_EventHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndSupress(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeEventListener eventListener)
        {
            // Arrange

            var eventHandler = new FakeEventHandler
            {
                ThrowsException = true,
                HasInnerException = true
            };

            var decorator = new EventLoggerDecorator<FakeEventListener>(
                () => eventHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(eventListener)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.FromSeconds(1));

            // Act

            decorator.Handle(eventListener);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing event \"FakeEventHandler\" with the listener parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeEventHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Inner exception caught: FakeEventHandlerInnerException."), Times.Once);
            logger.Verify(l => l.Log("Event \"FakeEventHandler\" executed in 00:00:01."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }
    }
}