using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class QueryLifetimeScopeDecoratorTests
    {
        [Theory, AutoMoqData]
        public void Handle_HasCurrentLifetimeScope_ShouldHandleQuery(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IQueryHandler<IQuery<object>, object>> queryHandler,
            IQuery<object> query,
            QueryLifetimeScopeDecorator<IQuery<object>, object> decorator)
        {
            // Act

            var result = decorator.Handle(query);

            // Assert

            result.Should().Not.Be(null);

            queryHandler.Verify(h => h.Handle(query), Times.Once);

            container.Verify(c => c.BeginLifetimeScope(), Times.Never);
        }

        [Theory, AutoMoqData]
        public void Handle_HasNoCurrentLifetimeScope_ShouldBeginNewLifetimeScopeBeforeHandleQuery(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IQueryHandler<IQuery<object>, object>> queryHandler,
            IQuery<object> query,
            QueryLifetimeScopeDecorator<IQuery<object>, object> decorator)
        {
            // Arrange

            var callOrder = 0;

            container.Setup(c => c.GetCurrentLifetimeScope()).Returns(null);
            container.Setup(c => c.BeginLifetimeScope()).Callback(() => (callOrder++).Should().Be(0));
            queryHandler.Setup(c => c.Handle(query)).Callback(() => (callOrder++).Should().Be(1));

            // Act

            decorator.Handle(query);

            // Assert

            container.Verify(c => c.BeginLifetimeScope(), Times.Once);

            queryHandler.Verify(h => h.Handle(query), Times.Once);
        }
    }
}