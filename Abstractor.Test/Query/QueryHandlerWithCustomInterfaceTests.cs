using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Query
{
    public class QueryHandlerWithCustomInterfaceTests : BaseTest
    {
        // Interface defined after the query handler
        public class Fake1Repository : IQueryHandler<Fake1Query, FakeResult>, IFake1Repository
        {
            public FakeResult Handle(Fake1Query query)
            {
                return new FakeResult();
            }
        }

        // Interface defined before the query handler
        public class Fake2Repository : IFake2Repository, IQueryHandler<Fake2Query, FakeResult>
        {
            public FakeResult Handle(Fake2Query query)
            {
                return new FakeResult();
            }
        }

        public interface IFake1Repository
        {
        }

        public interface IFake2Repository
        {
        }

        public class Fake1Query : IQuery<FakeResult>
        {
        }

        public class Fake2Query : IQuery<FakeResult>
        {
        }

        public class FakeResult
        {
        }

        [Fact]
        public void QueryDispatcher_InterfaceAfter_ShouldHandle()
        {
            // Arrange

            var query = new Fake2Query();

            // Act and assert

            var result = QueryDispatcher.Dispatch(query);

            result.Should().Not.Be.Null();
        }

        [Fact]
        public void QueryDispatcher_InterfaceBefore_ShouldHandle()
        {
            // Arrange

            var query = new Fake1Query();

            // Act and assert

            var result = QueryDispatcher.Dispatch(query);

            result.Should().Not.Be.Null();
        }
    }
}