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
            public string Property { get; } = "Value";
        }

        public class FakeResult
        {
        }

        [Theory, AutoMoqData]
        public void Handle_Success_LoggerShouldBeCalled2TimesBeforeQueryHandlerAnd1TimeAfter(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IQueryHandler<FakeQuery, FakeResult>> queryHandler,
            FakeQuery query,
            QueryLoggerDecorator<FakeQuery, FakeResult> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing query \"FakeQuery\" with the parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            queryHandler.Setup(h => h.Handle(query)).Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Query \"FakeQuery\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act

            decorator.Handle(query);
        }

        [Theory, AutoMoqData]
        public void Handle_ThrowsExceptionOnSerialize_ShouldLogTheException(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IQueryHandler<FakeQuery, FakeResult>> queryHandler,
            FakeQuery query,
            QueryLoggerDecorator<FakeQuery, FakeResult> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing query \"FakeQuery\" with the parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Throws(new Exception("Serialization exception."));

            logger.Setup(
                l => l.Log(It.Is<string>(s => s == "Could not serialize the parameters: Serialization exception.")))
                .Callback(() => { callOrder++.Should().Be(1); });

            queryHandler.Setup(h => h.Handle(query)).Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Query \"FakeQuery\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act

            decorator.Handle(query);
        }

        [Theory, AutoMoqData]
        public void Handle_QueryHandlerThrowsException_ShouldLogTheExceptionAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IQueryHandler<FakeQuery, FakeResult>> queryHandler,
            FakeQuery query,
            QueryLoggerDecorator<FakeQuery, FakeResult> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing query \"FakeQuery\" with the parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            queryHandler.Setup(h => h.Handle(query)).Throws(new Exception("QueryHandler exception."));

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Exception caught: QueryHandler exception.")))
                .Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Query \"FakeQuery\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(3); });

            // Act and assert

            var ex = Assert.Throws<Exception>(() => decorator.Handle(query));
            ex.Message.Should().Be("QueryHandler exception.");
        }

        [Theory, AutoMoqData]
        public void Handle_QueryHandlerThrowsExceptionWithInnerException_ShouldLogTheExceptionsAndRethrow(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IQueryHandler<FakeQuery, FakeResult>> queryHandler,
            FakeQuery query,
            QueryLoggerDecorator<FakeQuery, FakeResult> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Executing query \"FakeQuery\" with the parameters:")))
                .Callback(() => { callOrder++.Should().Be(0); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "{\r\n  \"Property\": \"Value\"\r\n}")))
                .Callback(() => { callOrder++.Should().Be(1); });

            queryHandler.Setup(h => h.Handle(query))
                .Throws(new Exception("QueryHandler exception.", new Exception("Inner exception.")));

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Exception caught: QueryHandler exception.")))
                .Callback(() => { callOrder++.Should().Be(2); });

            logger.Setup(l => l.Log(It.Is<string>(s => s == "Inner exception caught: Inner exception.")))
                .Callback(() => { callOrder++.Should().Be(3); });

            logger.Setup(l => l.Log(It.Is<string>(s => s.StartsWith("Query \"FakeQuery\" executed in"))))
                .Callback(() => { callOrder++.Should().Be(4); });

            // Act and assert

            var ex = Assert.Throws<Exception>(() => decorator.Handle(query));

            ex.Message.Should().Be("QueryHandler exception.");
            ex.InnerException.Message.Should().Be("Inner exception.");
        }
    }
}