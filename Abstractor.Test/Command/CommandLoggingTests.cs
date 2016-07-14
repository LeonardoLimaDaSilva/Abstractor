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
    public class CommandLoggingTests : BaseTest
    {
        public class LoggedCommand : ICommand
        {
        }

        public class TransactionalLoggedCommand : ICommand
        {
        }

        public class TransactionalNonLoggedCommand : ICommand
        {
        }

        public class NonTransactionalNonLoggedCommand : ICommand
        {
        }

        [Log]
        public class LoggedCommandCommandHandler : ICommandHandler<LoggedCommand>
        {
            public IEnumerable<IDomainEvent> Handle(LoggedCommand command)
            {
                yield break;
            }
        }

        [Log]
        [Transactional]
        public class TransactionalLoggedCommandHandler : ICommandHandler<TransactionalLoggedCommand>
        {
            public IEnumerable<IDomainEvent> Handle(TransactionalLoggedCommand command)
            {
                yield break;
            }
        }

        [Transactional]
        public class TransactionalNonLoggedCommandHandler : ICommandHandler<TransactionalNonLoggedCommand>
        {
            public IEnumerable<IDomainEvent> Handle(TransactionalNonLoggedCommand command)
            {
                yield break;
            }
        }

        public class NonTransactionalNonLoggedCommandHandler : ICommandHandler<NonTransactionalNonLoggedCommand>
        {
            public IEnumerable<IDomainEvent> Handle(NonTransactionalNonLoggedCommand command)
            {
                yield break;
            }
        }

        [Fact]
        public void Dispatch_CommandDecoratedWithLog_ShouldLogCommand()
        {
            // Arrange

            Logger.SetUp();

            // Act

            CommandDispatcher.Dispatch(new LoggedCommand());

            // Assert

            Logger.MessagesShouldBe(
                "Executing command \"LoggedCommand\" with the parameters:",
                "{}",
                "Command \"LoggedCommand\" executed in 00:00:00.");
        }

        [Fact]
        public void Dispatch_CommandDecoratedWithLogAndTransactional_ShouldLogCommandAndTransaction()
        {
            // Arrange

            Logger.SetUp();

            // Act

            CommandDispatcher.Dispatch(new TransactionalLoggedCommand());

            // Assert

            Logger.MessagesShouldBe(
                "Executing command \"TransactionalLoggedCommand\" with the parameters:",
                "{}",
                "Starting transaction...",
                "Transaction committed.",
                "Command \"TransactionalLoggedCommand\" executed in 00:00:00.");
        }

        [Fact]
        public void Dispatch_CommandDecoratedWithTransactionalOnly_ShouldNotLog()
        {
            // Arrange

            Logger.SetUp();

            // Act

            CommandDispatcher.Dispatch(new TransactionalNonLoggedCommand());

            // Assert

            Logger.MessagesShouldBeEmpty();
        }

        [Fact]
        public void Dispatch_CommandNotDecorated_ShouldNotLog()
        {
            // Arrange

            Logger.SetUp();

            // Act

            CommandDispatcher.Dispatch(new NonTransactionalNonLoggedCommand());

            // Assert

            Logger.MessagesShouldBeEmpty();
        }

        [Fact]
        public void DispatchAsync_CommandDecoratedWithLog_ShouldLogCommand()
        {
            // Arrange

            Logger.SetUp();

            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(async () =>
            {
                // Act

                await CommandDispatcher.DispatchAsync(new LoggedCommand());

                // Assert

                Logger.MessagesShouldBe(
                    "Executing command \"LoggedCommand\" with the parameters:",
                    "{}",
                    "Command \"LoggedCommand\" executed in 00:00:00.");
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);
        }
    }
}