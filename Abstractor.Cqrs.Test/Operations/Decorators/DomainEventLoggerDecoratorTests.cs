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
    public class DomainEventLoggerDecoratorTests
    {
        public class FakeDomainEvent : IDomainEvent
        {
        }

        public class FakeEventHandler : IDomainEventHandler<FakeDomainEvent>
        {
            public bool Executed { get; private set; }

            public bool ThrowsException { get; set; }

            public bool HasInnerException { get; set; }

            public void Handle(FakeDomainEvent domainEvent)
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
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(domainEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing domain event \"FakeDomainEvent\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Domain event \"FakeDomainEvent\" executed in 00:00:00."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsOnSerialize_ShouldLogException(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            loggerSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
                            .Throws(new Exception("Serialization exception."));

            // Act

            decorator.Handle(domainEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing domain event \"FakeDomainEvent\" with the parameters:"), Times.Once);
            logger.Verify(
                l => l.Log("Could not serialize the parameters: Serialization exception."),
                Times.Once);
            logger.Verify(l => l.Log("Domain event \"FakeDomainEvent\" executed in 00:00:00."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_EventHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler { ThrowsException = true };

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(domainEvent));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing domain event \"FakeDomainEvent\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeEventHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Domain event \"FakeDomainEvent\" executed in 00:00:00."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_EventHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler
            {
                ThrowsException = true,
                HasInnerException = true
            };

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object);

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(domainEvent));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once());
            stopwatch.Verify(s => s.Stop(), Times.Once());

            logger.Verify(l => l.Log("Executing domain event \"FakeDomainEvent\" with the parameters:"), Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeEventHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Inner exception caught: FakeEventHandlerInnerException."), Times.Once);
            logger.Verify(l => l.Log("Domain event \"FakeDomainEvent\" executed in 00:00:00."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }
    }
}