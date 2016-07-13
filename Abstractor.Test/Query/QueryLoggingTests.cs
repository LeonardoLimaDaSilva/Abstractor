using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using Xunit;

namespace Abstractor.Test.Query
{
    public class QueryLoggingTests : BaseTest
    {
        public class FakeResult
        {
        }

        public class LoggedQuery : IQuery<FakeResult>
        {
        }

        public class NonLoggedQuery : IQuery<FakeResult>
        {
        }

        [Log]
        public class LoggedQueryHandler : IQueryHandler<LoggedQuery, FakeResult>
        {
            public FakeResult Handle(LoggedQuery query)
            {
                return new FakeResult();
            }
        }

        [Log]
        public class LoggedQueryAsyncHandler : IQueryAsyncHandler<LoggedQuery, FakeResult>
        {
            public async Task<FakeResult> HandleAsync(LoggedQuery query)
            {
                return await Task.Run(() => new FakeResult());
            }
        }

        public class NonLoggedQueryHandler : IQueryHandler<NonLoggedQuery, FakeResult>
        {
            public FakeResult Handle(NonLoggedQuery query)
            {
                return new FakeResult();
            }
        }

        public class NonLoggedQueryAsyncHandler : IQueryAsyncHandler<NonLoggedQuery, FakeResult>
        {
            public async Task<FakeResult> HandleAsync(NonLoggedQuery query)
            {
                return await Task.Run(() => new FakeResult());
            }
        }

        [Fact]
        public void Dispatch_QueryDecoratedWithLog_ShouldLog()
        {
            // Arrange

            Logger.SetUp();

            // Act

            QueryDispatcher.Dispatch(new LoggedQuery());

            // Assert

            Logger.MessagesShouldBe(
                "Executing query \"LoggedQuery\" with the parameters:",
                "{}",
                "Query \"LoggedQuery\" executed in 00:00:00.");
        }

        [Fact]
        public void Dispatch_QueryNotDecoratedWithLog_ShouldNotLog()
        {
            // Arrange

            Logger.SetUp();

            // Act

            QueryDispatcher.Dispatch(new NonLoggedQuery());

            // Assert

            Logger.MessagesShouldBeEmpty();
        }

        [Fact]
        public void DispatchAsync_QueryDecoratedWithLog_ShouldLog()
        {
            // Arrange

            Logger.SetUp();

            // Act

            QueryDispatcher.DispatchAsync(new LoggedQuery());

            // Assert

            Logger.MessagesShouldBe(
                "Executing query \"LoggedQuery\" with the parameters:",
                "{}",
                "Query \"LoggedQuery\" executed in 00:00:00.");
        }

        [Fact]
        public void DispatchAsync_QueryNotDecoratedWithLog_ShouldNotLog()
        {
            // Arrange

            Logger.SetUp();

            // Act

            QueryDispatcher.DispatchAsync(new NonLoggedQuery());

            // Assert

            Logger.MessagesShouldBeEmpty();
        }
    }
}