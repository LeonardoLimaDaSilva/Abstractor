using System;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Dispatchers
{
    public class QueryDispatcherTests
    {
        [Theory, AutoMoqData]
        public void Dispatch_NullQuery_ThrowsArgumentNullException(QueryDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => dispatcher.Dispatch((IQuery<object>) null));
        }

        [Theory, AutoMoqData]
        public void Dispatch_BuildGenericQueryHandlerAndGetFromContainer_ShouldHandleQuery(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IQueryHandler<FakeQuery, object>> queryHandler,
            FakeQuery query,
            QueryDispatcher dispatcher)
        {
            // Arrange

            var genericTypeName = typeof(IQueryHandler<FakeQuery, object>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                .Returns(queryHandler.Object);

            // Act

            var result = dispatcher.Dispatch(query);

            // Assert

            result.Should().Not.Be(null);

            queryHandler.Verify(t => t.Handle(query), Times.Once);
        }

        [Theory, AutoMoqData]
        public async void DispatchAsync_NullQuery_ThrowsArgumentNullException(QueryDispatcher dispatcher)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => dispatcher.DispatchAsync((IQuery<object>) null));
        }

        [Theory, AutoMoqData]
        public async void DispatchAsync_BuildGenericQueryHandlerAndGetFromContainer_ShouldHandleQueryOnANewThread(
            [Frozen] Mock<IContainer> container,
            [Frozen] Mock<IQueryAsyncHandler<FakeQuery, object>> queryHandler,
            FakeQuery query,
            QueryDispatcher dispatcher)
        {
            // Arrange

            var genericTypeName = typeof(IQueryAsyncHandler<FakeQuery, object>).FullName;

            container.Setup(c => c.GetInstance(It.Is<Type>(t => t.FullName == genericTypeName)))
                .Returns(queryHandler.Object);

            // Act

            await dispatcher.DispatchAsync(query);

            // Assert

            queryHandler.Verify(t => t.HandleAsync(query), Times.Once);
        }

        public class FakeQuery : IQuery<object>
        {
        }
    }
}