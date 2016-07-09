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
    //TODO Verificar por que quando roda todos os testes, esse falha apenas na segunda vez
    public class EventLoggerDecoratorTests
    {
        public class FakeEventListener : IEventListener
        {
            public string Property { get; } = "Value";
        }

        [Theory, AutoMoqData]
        public void Handle_Success_LoggerShouldBeCalled2TimesBeforeEventHandlerAnd1TimeAfter(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEventHandler<FakeEventListener>> eventHandler,
            FakeEventListener eventListener,
            EventLoggerDecorator<FakeEventListener> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing event \"IEventHandler`1Proxy\" with the listener parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            eventHandler.Setup(h => h.Handle(eventListener)).Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Event \"IEventHandler`1Proxy\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act

            decorator.Handle(eventListener);
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsExceptionOnSerialize_ShouldLogTheException(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEventHandler<FakeEventListener>> eventHandler,
            FakeEventListener eventListener,
            EventLoggerDecorator<FakeEventListener> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing event \"IEventHandler`1Proxy\" with the listener parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Throws(new Exception("Serialization exception."));

            logger.Setup(
                l => l.Log(It.Is<string>(s => s == "Could not serialize the listener parameters: Serialization exception.")))
                .Callback(() => { callOrder++.Should().Be(1); });

            eventHandler.Setup(h => h.Handle(eventListener)).Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Event \"IEventHandler`1Proxy\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act

            decorator.Handle(eventListener);
        }

        [Theory, AutoMoqData]
        public void Handle_EventHandlerThrowsException_ShouldLogTheExceptionAndSupress(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEventHandler<FakeEventListener>> eventHandler,
            FakeEventListener eventListener,
            EventLoggerDecorator<FakeEventListener> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing event \"IEventHandler`1Proxy\" with the listener parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            eventHandler.Setup(h => h.Handle(eventListener)).Throws(new Exception("EventHandler exception."));

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Exception caught: EventHandler exception.")))
                .Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Event \"IEventHandler`1Proxy\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act

            decorator.Handle(eventListener);
        }

        [Theory, AutoMoqData]
        public void Handle_EventHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndSupress(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEventHandler<FakeEventListener>> eventHandler,
            FakeEventListener eventListener,
            EventLoggerDecorator<FakeEventListener> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing event \"IEventHandler`1Proxy\" with the listener parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            eventHandler.Setup(h => h.Handle(eventListener))
                .Throws(new Exception("EventHandler exception.", new Exception("Inner exception.")));

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Exception caught: EventHandler exception.")))
                .Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Inner exception caught: Inner exception.")))
                .Callback(() => { callOrder++.Should().Be(3); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Event \"IEventHandler`1Proxy\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(4); });

            // Act

            decorator.Handle(eventListener);
        }
    }
}