using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using Xunit;

namespace Abstractor.Test.Command
{
    public class CommandTransactionTests : BaseTest
    {
        public class TransactionalCommand : ICommand
        {
        }

        public class NonTransactionalCommand : ICommand
        {
        }

        [Transaction]
        public class TransactionalCommandHandler : ICommandHandler<TransactionalCommand>
        {
            public void Handle(TransactionalCommand command)
            {
            }
        }

        public class NonTransactionalCommandHandler : ICommandHandler<NonTransactionalCommand>
        {
            public void Handle(NonTransactionalCommand command)
            {
            }
        }

        [Fact]
        public void HandlerDecoratedWithTransaction_ShouldCommit()
        {
            using (Logger)
            using (UnitOfWork)
            {
                // Arrange

                var command = new TransactionalCommand();

                // Act

                CommandDispatcher.Dispatch(command);

                // Assert

                UnitOfWork.CommittedShouldBe(true);

                Logger.MessagesShouldBe(
                    "Executing command \"TransactionalCommand\" with the parameters:",
                    "{}",
                    "Starting transactional command.",
                    "Transaction committed successfully.",
                    "Command \"TransactionalCommand\" executed in 00:00:00.");
            }
        }

        [Fact]
        public void HandlerNotDecoratedWithTransaction_ShouldNotCommit()
        {
            using (Logger)
            using (UnitOfWork)
            {
                // Arrange

                var command = new NonTransactionalCommand();

                // Act

                CommandDispatcher.Dispatch(command);

                // Assert

                UnitOfWork.CommittedShouldBe(false);

                Logger.MessagesShouldBe(
                    "Executing command \"NonTransactionalCommand\" with the parameters:",
                    "{}",
                    "Command \"NonTransactionalCommand\" executed in 00:00:00.");
            }
        }
    }
}