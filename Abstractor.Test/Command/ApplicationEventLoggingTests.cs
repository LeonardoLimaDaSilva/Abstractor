using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using Xunit;

namespace Abstractor.Test.Command
{
    public class ApplicationEventLoggingTests : BaseTest
    {
        public class FakeCommand : ICommand, IApplicationEvent
        {
        }

        public class FakeCommandHandler : ICommandHandler<FakeCommand>
        {
            public IEnumerable<IDomainEvent> Handle(FakeCommand command)
            {
                yield break;
            }
        }

        [Log]
        public class LoggedCommandHandled1 : IApplicationEventHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
            }
        }

        [Log]
        public class LoggedCommandHandled2 : IApplicationEventHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
            }
        }

        public class NonLoggedCommandHandled : IApplicationEventHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
            }
        }

        [Fact]
        public void TwoOfThreeEventHandlersDecoratedWithLog_ShouldLogOnlyTheTwoEvents()
        {
            // Arrange

            Logger.SetUp();

            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(() =>
            {
                // Act

                CommandDispatcher.Dispatch(new FakeCommand());
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            Logger.MessagesShouldBe(
                "Executing event \"LoggedCommandHandled1\" with the parameters:",
                "{}",
                "Event \"LoggedCommandHandled1\" executed in 00:00:00.",
                "Executing event \"LoggedCommandHandled2\" with the parameters:",
                "{}",
                "Event \"LoggedCommandHandled2\" executed in 00:00:00.");
        }
    }
}