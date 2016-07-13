using Abstractor.Cqrs.Infrastructure.Operations;
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

        [Transactional]
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
            // Arrange

            UnitOfWork.SetUp();

            var command = new TransactionalCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            UnitOfWork.CommittedShouldBe(true);
        }

        [Fact]
        public void HandlerNotDecoratedWithTransaction_ShouldNotCommit()
        {
            // Arrange

            UnitOfWork.SetUp();

            var command = new NonTransactionalCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            UnitOfWork.CommittedShouldBe(false);
        }
    }
}