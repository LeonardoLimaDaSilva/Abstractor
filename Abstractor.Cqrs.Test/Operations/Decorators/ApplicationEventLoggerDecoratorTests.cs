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
    public class ApplicationEventLoggerDecoratorTests
    {
        public class FakeApplicationEvent : IApplicationEvent
        {
        }

        public class FakeEventHandler : IApplicationEventHandler<FakeApplicationEvent>
        {
            public bool Executed { get; private set; }

            public bool ThrowsException { get; set; }

            public bool HasInnerException { get; set; }

            public void Handle(FakeApplicationEvent applicationEvent)
            {
                Executed = true;

                if (!ThrowsException) return;

                if (!HasInnerException) throw new Exception("FakeEventHandlerException.");

                throw new Exception("FakeEventHandlerException.", new Exception("FakeEventHandlerInnerException."));
            }
        }

        [Theory, AutoMoqData]
        public void Handle_WithoutLogAttribute_ShouldNotLog(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeApplicationEvent applicationEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();

            var decorator = new ApplicationEventLoggerDecorator<FakeApplicationEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object,
                new GlobalSettings());

            loggerSerializer.Setup(s => s.Serialize(applicationEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(applicationEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Never);
            stopwatch.Verify(s => s.Stop(), Times.Never());

            logger.Verify(l => l.Log(It.IsAny<string>()), Times.Never);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_WithoutLogAttributeAndGloballyEnabled_ShouldLog(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeApplicationEvent applicationEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();

            var decorator = new ApplicationEventLoggerDecorator<FakeApplicationEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object,
                new GlobalSettings
                {
                    EnableLogging = true
                });

            loggerSerializer.Setup(s => s.Serialize(applicationEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(applicationEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log(It.IsAny<string>()), Times.AtLeastOnce);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_Success_ShouldLogMessagesAndCallMethods(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeApplicationEvent applicationEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();

            var decorator = new ApplicationEventLoggerDecorator<FakeApplicationEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(applicationEvent.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(applicationEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            decorator.Handle(applicationEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing application event \"FakeEventHandler\" with the parameters:"),
                Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Application event \"FakeEventHandler\" executed in 00:00:00."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsOnSerialize_ShouldLogException(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeApplicationEvent applicationEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler();

            var decorator = new ApplicationEventLoggerDecorator<FakeApplicationEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(applicationEvent.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(applicationEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            loggerSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
                            .Throws(new Exception("Serialization exception."));

            // Act

            decorator.Handle(applicationEvent);

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing application event \"FakeEventHandler\" with the parameters:"),
                Times.Once);
            logger.Verify(
                l => l.Log("Could not serialize the parameters: Serialization exception."),
                Times.Once);
            logger.Verify(l => l.Log("Application event \"FakeEventHandler\" executed in 00:00:00."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_EventHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeApplicationEvent applicationEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler {ThrowsException = true};

            var decorator = new ApplicationEventLoggerDecorator<FakeApplicationEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(applicationEvent.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(applicationEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(applicationEvent));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing application event \"FakeEventHandler\" with the parameters:"),
                Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeEventHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Application event \"FakeEventHandler\" executed in 00:00:00."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }

        [Theory, AutoMoqData]
        public void Handle_EventHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IStopwatch> stopwatch,
            [Frozen] Mock<ILoggerSerializer> loggerSerializer,
            FakeApplicationEvent applicationEvent)
        {
            // Arrange

            var eventHandler = new FakeEventHandler
            {
                ThrowsException = true,
                HasInnerException = true
            };

            var decorator = new ApplicationEventLoggerDecorator<FakeApplicationEvent>(
                () => eventHandler,
                attributeFinder.Object,
                stopwatch.Object,
                loggerSerializer.Object,
                logger.Object,
                new GlobalSettings());

            attributeFinder.Setup(f => f.Decorates(applicationEvent.GetType(), typeof (LogAttribute))).Returns(true);

            loggerSerializer.Setup(s => s.Serialize(applicationEvent)).Returns("Serialized parameters");

            stopwatch.Setup(s => s.GetElapsed()).Returns(TimeSpan.Zero);

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(applicationEvent));

            // Assert

            stopwatch.Verify(s => s.Start(), Times.Once);
            stopwatch.Verify(s => s.Stop(), Times.Once);

            logger.Verify(l => l.Log("Executing application event \"FakeEventHandler\" with the parameters:"),
                Times.Once);
            logger.Verify(l => l.Log("Serialized parameters"), Times.Once);
            logger.Verify(l => l.Log("Exception caught: FakeEventHandlerException."), Times.Once);
            logger.Verify(l => l.Log("Inner exception caught: FakeEventHandlerInnerException."), Times.Once);
            logger.Verify(l => l.Log("Application event \"FakeEventHandler\" executed in 00:00:00."), Times.Once);

            eventHandler.Executed.Should().Be.True();
        }
    }
}