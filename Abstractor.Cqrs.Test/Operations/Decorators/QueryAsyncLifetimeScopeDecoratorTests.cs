using System.Threading.Tasks;
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
    public class QueryAsyncLifetimeScopeDecoratorTests
    {
        [Theory, AutoMoqData]
        public void Handle_HasCurrentLifetimeScope_ShouldHandleQuery(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IQueryAsyncHandler<IQuery<object>, object>> queryHandler,
            IQuery<object> query,
            Task<object> queryResult,
            QueryAsyncLifetimeScopeDecorator<IQuery<object>, object> decorator)
        {
            // Arrange

            queryHandler.Setup(h => h.HandleAsync(query)).Returns(queryResult);

            // Act

            var result = decorator.HandleAsync(query).Result;

            // Assert

            result.Should().Not.Be(null);

            queryHandler.Verify(h => h.HandleAsync(query), Times.Once);

            container.Verify(c => c.BeginLifetimeScope(), Times.Never);
        }

        [Theory, AutoMoqData]
        public void Handle_HasNoCurrentLifetimeScope_ShouldBeginNewLifetimeScopeBeforeHandleQuery(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IQueryAsyncHandler<IQuery<object>, object>> queryHandler,
            IQuery<object> query,
            QueryAsyncLifetimeScopeDecorator<IQuery<object>, object> decorator)
        {
            // Arrange

            var callOrder = 0;

            container.Setup(c => c.GetCurrentLifetimeScope()).Returns(null);
            container.Setup(c => c.BeginLifetimeScope()).Callback(() => callOrder++.Should().Be(0));
            queryHandler.Setup(c => c.HandleAsync(query)).Callback(() => callOrder++.Should().Be(1));

            // Act

            decorator.HandleAsync(query);

            // Assert

            container.Verify(c => c.BeginLifetimeScope(), Times.Once);

            queryHandler.Verify(h => h.HandleAsync(query), Times.Once);
        }
    }
}