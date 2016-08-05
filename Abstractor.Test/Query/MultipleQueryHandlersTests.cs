using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Query
{
    public class MultipleQueryHandlersTests : BaseTest
    {
        // Classes should be able to handle multiple queries of distinct types,
        // E.g. repositories should be able to handle an optimized query and return the results 
        // directly to the consumer via tailored DTOs
        public class MultipleQueryHandler : 
            IQueryHandler<FakeQuery1, FakeQuery1Result>,
            IQueryHandler<FakeQuery2, FakeQuery2Result>,
            IQueryHandler<FakeQuery3, FakeCommonResult>,
            IQueryHandler<FakeQuery4, FakeCommonResult>,
            IQueryAsyncHandler<FakeQuery1, FakeQuery1Result>,
            IQueryAsyncHandler<FakeQuery2, FakeQuery2Result>,
            IQueryAsyncHandler<FakeQuery3, FakeCommonResult>,
            IQueryAsyncHandler<FakeQuery4, FakeCommonResult>
        {
            public FakeQuery1Result Handle(FakeQuery1 query)
            {
                return new FakeQuery1Result();
            }

            public FakeQuery2Result Handle(FakeQuery2 query)
            {
                return new FakeQuery2Result();
            }

            public FakeCommonResult Handle(FakeQuery3 query)
            {
                return new FakeCommonResult();
            }

            public FakeCommonResult Handle(FakeQuery4 query)
            {
                return new FakeCommonResult();
            }

            public async Task<FakeQuery1Result> HandleAsync(FakeQuery1 query)
            {
                return await Task.Run(() => new FakeQuery1Result());
            }

            public async Task<FakeQuery2Result> HandleAsync(FakeQuery2 query)
            {
                return await Task.Run(() => new FakeQuery2Result());
            }

            public async Task<FakeCommonResult> HandleAsync(FakeQuery3 query)
            {
                return await Task.Run(() => new FakeCommonResult());
            }

            public async Task<FakeCommonResult> HandleAsync(FakeQuery4 query)
            {
                return await Task.Run(() => new FakeCommonResult());
            }
        }

        // Duplicate implementations of a query handler are not permitted
        public class Fake2Repository : 
            IQueryHandler<FakeQuery1, FakeQuery1Result>,
            IQueryAsyncHandler<FakeQuery1, FakeQuery1Result>
        {
            public FakeQuery1Result Handle(FakeQuery1 query)
            {
                return new FakeQuery1Result();
            }

            public async Task<FakeQuery1Result> HandleAsync(FakeQuery1 query)
            {
                return await Task.Run(() => new FakeQuery1Result());
            }
        }

        public class FakeQuery1 : IQuery<FakeQuery1Result>
        {
        }

        public class FakeQuery2 : IQuery<FakeQuery2Result>
        {
        }

        public class FakeQuery3 : IQuery<FakeCommonResult>
        {
        }

        public class FakeQuery4 : IQuery<FakeCommonResult>
        {
        }

        public class FakeQueryWithNoHandlers : IQuery<FakeCommonResult>
        {
        }

        public class FakeQuery1Result
        {
        }

        public class FakeQuery2Result
        {
        }

        public class FakeCommonResult
        {
        }

        [Fact]
        public void Dispatch_FakeQuery1HandledByMultipleRepositories_ShouldThrowException()
        {
            // Arrange

            var query = new FakeQuery1();

            // Act and assert

            Assert.Throws<MultipleQueryHandlersException>(() => QueryDispatcher.Dispatch(query));
        }

        [Fact]
        public void Dispatch_FakeQuery2_ShouldBeHandled()
        {
            // Arrange

            var query = new FakeQuery2();

            // Act

            var result = QueryDispatcher.Dispatch(query);

            // Assert

            result.Should().Not.Be.Null();
        }

        [Fact]
        public void Dispatch_FakeQuery3_SameResultTypeOfFakeQuery4_ShouldBeHandled()
        {
            // Arrange

            var query = new FakeQuery3();

            // Act

            var result = QueryDispatcher.Dispatch(query);

            // Assert

            result.Should().Not.Be.Null();
        }

        [Fact]
        public void Dispatch_FakeQuery4_SameResultTypeOfFakeQuery3_ShouldBeHandled()
        {
            // Arrange

            var query = new FakeQuery4();

            // Act

            var result = QueryDispatcher.Dispatch(query);

            // Assert

            result.Should().Not.Be.Null();
        }

        [Fact]
        public void Dispatch_FakeQueryWithNoHandlers_ShouldThrowException()
        {
            // Arrange

            var query = new FakeQueryWithNoHandlers();

            // Act and assert

            Assert.Throws<QueryHandlersNotFoundException>(() => QueryDispatcher.Dispatch(query));
        }

        [Fact]
        public async void DispatchAsync_FakeQuery1HandledByMultipleRepositories_ShouldThrowException()
        {
            // Arrange

            var query = new FakeQuery1();

            // Act and assert

            await Assert.ThrowsAsync<MultipleQueryHandlersException>(() => QueryDispatcher.DispatchAsync(query));
        }

        [Fact]
        public async void DispatchAsync_FakeQuery2_ShouldBeHandled()
        {
            // Arrange

            var query = new FakeQuery2();

            // Act

            var result = await QueryDispatcher.DispatchAsync(query);

            // Assert

            result.Should().Not.Be.Null();
        }

        [Fact]
        public async void DispatchAsync_FakeQuery3_SameResultTypeOfFakeQuery4_ShouldBeHandled()
        {
            // Arrange

            var query = new FakeQuery3();

            // Act

            var result = await QueryDispatcher.DispatchAsync(query);

            // Assert

            result.Should().Not.Be.Null();
        }

        [Fact]
        public async void DispatchAsync_FakeQuery4_SameResultTypeOfFakeQuery3_ShouldBeHandled()
        {
            // Arrange

            var query = new FakeQuery4();

            // Act

            var result = await QueryDispatcher.DispatchAsync(query);

            // Assert

            result.Should().Not.Be.Null();
        }

        [Fact]
        public async void DispatchAsync_FakeQueryWithNoHandlers_ShouldThrowException()
        {
            // Arrange

            var query = new FakeQueryWithNoHandlers();

            // Act and assert

            await Assert.ThrowsAsync<QueryHandlersNotFoundException>(() => QueryDispatcher.DispatchAsync(query));
        }
    }
}