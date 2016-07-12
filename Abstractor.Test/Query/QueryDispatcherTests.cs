using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Query
{
    public class QueryDispatcherTests : BaseTest
    {
        public class FakeQuery : IQuery<FakeResult>
        {
            public bool HandlerExecuted { get; set; }
        }

        public class FakeResult
        {
        }

        public class FakeQueryHandler : IQueryHandler<FakeQuery, FakeResult>
        {
            public FakeResult Handle(FakeQuery query)
            {
                query.HandlerExecuted = true;

                return new FakeResult();
            }
        }

        public class FakeQueryAsyncHandler : IQueryAsyncHandler<FakeQuery, FakeResult>
        {
            public async Task<FakeResult> HandleAsync(FakeQuery query)
            {
                query.HandlerExecuted = true;

                return await Task<FakeResult>.Factory.StartNew(() => new FakeResult());
            }
        }

        [Fact]
        public void Dispatch_ShouldExecuteHandler()
        {
            // Arrange

            var query = new FakeQuery();

            // Act

            var result = QueryDispatcher.Dispatch(query);

            // Assert

            query.HandlerExecuted.Should().Be.True();

            result.Should().Not.Be.Null();
        }

        [Fact]
        public void DispatchAsync_DoNotAwait_ShouldEnsureThatQueryWasDispatchedAsyncly()
        {
            // Arrange

            var query = new FakeQuery();

            // Act

            var result = QueryDispatcher.DispatchAsync(query);

            // Assert

            //TODO test fails only when run alone
            //SharpTestsEx.AssertExceptionRanToCompletion Should Be Equal To WaitingForActivation.

            result.Status.Should().Be(TaskStatus.WaitingForActivation);

            query.HandlerExecuted.Should().Be.True();

            result.Result.Should().Not.Be.Null();
        }

        [Fact]
        public async void DispatchAsync_Await_ShouldExecuteHandler()
        {
            // Arrange

            var query = new FakeQuery();

            // Act

            var result = await QueryDispatcher.DispatchAsync(query);

            // Assert

            query.HandlerExecuted.Should().Be.True();

            result.Should().Not.Be.Null();
        }
    }
}