using System;
using Abstractor.Cqrs.Infrastructure.CompositionRoot;
using Abstractor.Cqrs.Infrastructure.Operations;
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

            public bool HasInnerException { get; set; }

            public bool ThrowsException { get; set; }

            public void Handle(FakeDomainEvent domainEvent)
            {
                Executed = true;

                if (!ThrowsException) return;

                if (!HasInnerException) throw new Exception("FakeEventHandlerException.");

                throw new Exception("FakeEventHandlerException.", new Exception("FakeEventHandlerInnerException."));
            }
        }

        [Theory]
        [AutoMoqData]
        public void Handle_EventHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler {ThrowsException = true};
            var logger = new FakeLogger();

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(domainEvent.GetType(), typeof(LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(domainEvent));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.VerifyMessages(
                "Executing domain event \"FakeDomainEvent\" with the parameters:",
                "Serialized parameters",
                "Exception caught: FakeEventHandlerException.",
                "Domain event \"FakeDomainEvent\" executed in 00:00:00.");

            eventHandler.Executed.Should().Be.True();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_EventHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
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

            var logger = new FakeLogger();

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(domainEvent.GetType(), typeof(LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(domainEvent));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.VerifyMessages(
                "Executing domain event \"FakeDomainEvent\" with the parameters:",
                "Serialized parameters",
                "Exception caught: FakeEventHandlerException.",
                "Inner exception caught: FakeEventHandlerInnerException.",
                "Domain event \"FakeDomainEvent\" executed in 00:00:00.");

            eventHandler.Executed.Should().Be.True();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_Success_ShouldLogMessagesAndCallMethods(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();
            var logger = new FakeLogger();

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(domainEvent.GetType(), typeof(LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(domainEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.VerifyMessages(
                "Executing domain event \"FakeDomainEvent\" with the parameters:",
                "Serialized parameters",
                "Domain event \"FakeDomainEvent\" executed in 00:00:00.");

            eventHandler.Executed.Should().Be.True();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_ThrowsOnSerialize_ShouldLogException(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();
            var logger = new FakeLogger();

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(domainEvent.GetType(), typeof(LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            loggerSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
                            .Throws(new Exception("Serialization exception."));

            // Act

            decorator.Handle(domainEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.VerifyMessages(
                "Executing domain event \"FakeDomainEvent\" with the parameters:",
                "Could not serialize the parameters: Serialization exception.",
                "Domain event \"FakeDomainEvent\" executed in 00:00:00.");

            eventHandler.Executed.Should().Be.True();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_WithoutLogAttribute_ShouldNotLog(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();
            var logger = new FakeLogger();

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings());

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(domainEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Never);
            stopwatch.Verify(s => s.Stop(), Times.Never);

            logger.ShouldNeverBeCalled();

            eventHandler.Executed.Should().Be.True();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_WithoutLogAttributeAndGloballyEnabled_ShouldLog(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeDomainEvent domainEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();
            var logger = new FakeLogger();

            var decorator = new DomainEventLoggerDecorator<FakeDomainEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                () => logger,
                new GlobalSettings
                {
                    EnableLogging = true
                });

            loggerSerializer.Setup(s => s.Serialize(domainEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(domainEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.ShouldBeCalled();

            eventHandler.Executed.Should().Be.True();
        }
    }
}