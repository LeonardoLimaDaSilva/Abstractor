using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Command
{
    public class DomainEventTests : BaseTest
    {
        public class NonTransactionalFakeCommand : ICommand
        {
            public bool EventHandler1Executed { get; set; }

            public bool EventHandler2Executed { get; set; }
        }

        public class NonTransactionalFakeCommandHandler : CommandHandler<NonTransactionalFakeCommand>
        {
            public override void Handle(NonTransactionalFakeCommand command)
            {
                Emit(new NonTransactionalFakeDomainEvent(command));
            }
        }

        public class NonTransactionalFakeDomainEvent : IDomainEvent
        {
            public NonTransactionalFakeCommand Command { get; set; }

            public NonTransactionalFakeDomainEvent(NonTransactionalFakeCommand command)
            {
                Command = command;
            }
        }

        public class OnNonTransactionalFakeDomainEvent1 : IDomainEventHandler<NonTransactionalFakeDomainEvent>
        {
            public void Handle(NonTransactionalFakeDomainEvent domainEvent)
            {
                domainEvent.Command.EventHandler1Executed = true;
            }
        }

        public class OnNonTransactionalFakeDomainEvent2 : IDomainEventHandler<NonTransactionalFakeDomainEvent>
        {
            public void Handle(NonTransactionalFakeDomainEvent domainEvent)
            {
                domainEvent.Command.EventHandler2Executed = true;
            }
        }

        [Transactional]
        public class TransactionalFakeCommand : ICommand
        {
            public bool EventHandler1Executed { get; set; }

            public bool EventHandler2Executed { get; set; }
        }

        public class TransactionalFakeCommandHandler : CommandHandler<TransactionalFakeCommand>
        {
            public override void Handle(TransactionalFakeCommand command)
            {
                Emit(new TransactionalFakeDomainEvent(command));
            }
        }

        public class TransactionalFakeDomainEvent : IDomainEvent
        {
            public TransactionalFakeCommand Command { get; set; }

            public TransactionalFakeDomainEvent(TransactionalFakeCommand command)
            {
                Command = command;
            }
        }

        public class OnTransactionalFakeDomainEvent1 : IDomainEventHandler<TransactionalFakeDomainEvent>
        {
            public void Handle(TransactionalFakeDomainEvent domainEvent)
            {
                domainEvent.Command.EventHandler1Executed = true;
            }
        }

        public class OnTransactionalFakeDomainEvent2 : IDomainEventHandler<TransactionalFakeDomainEvent>
        {
            public void Handle(TransactionalFakeDomainEvent domainEvent)
            {
                domainEvent.Command.EventHandler2Executed = true;
            }
        }

        [Fact]
        public void NonTransactional_EventHandlersShouldBeExecuted()
        {
            // Arrange

            UnitOfWork.SetUp();

            var command = new NonTransactionalFakeCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.EventHandler1Executed.Should().Be.True();

            command.EventHandler2Executed.Should().Be.True();

            UnitOfWork.CommittedShouldBe(false);
        }

        [Fact]
        public void Transactional_EventHandlersShouldBeExecutedBeforeAndCommitted()
        {
            // Arrange

            UnitOfWork.SetUp();

            var command = new TransactionalFakeCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.EventHandler1Executed.Should().Be.True();

            command.EventHandler2Executed.Should().Be.True();

            UnitOfWork.CommittedShouldBe(true);
        }
    }
}