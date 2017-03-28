using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using Xunit;

namespace Abstractor.Test.Command
{
    public class CommandHandlerWithCustomInterfaceTests : BaseTest
    {
        // Interface defined after the command handler
        public class Fake1Repository : CommandHandler<Fake1Command>, IFake1Repository
        {
            public override void Handle(Fake1Command command)
            {
            }
        }

        // Interface defined before the command handler
        public class Fake2Repository : CommandHandler<Fake2Command>, IFake2Repository
        {
            public override void Handle(Fake2Command command)
            {
            }
        }

        public interface IFake1Repository
        {
        }

        public interface IFake2Repository
        {
        }

        public class Fake1Command : ICommand
        {
        }

        public class Fake2Command : ICommand
        {
        }

        [Fact]
        public void CommandDispatcher_InterfaceAfter_ShouldHandle()
        {
            var command = new Fake2Command();

            CommandDispatcher.Dispatch(command);
        }

        [Fact]
        public void CommandDispatcher_InterfaceBefore_ShouldHandle()
        {
            var command = new Fake1Command();

            CommandDispatcher.Dispatch(command);
        }
    }
}