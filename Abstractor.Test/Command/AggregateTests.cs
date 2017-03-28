using System;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Command
{
    public class AggregateTests : BaseTest
    {
        public class FakeAggregate : AggregateRoot<Guid>
        {
            private readonly ExecuteOperation _command;

            public FakeAggregate(ExecuteOperation command)
                : base(Guid.Empty)
            {
                _command = command;
            }

            public void Execute()
            {
                if (!_command.AllowExecuteOperations) return;

                Emit(new OperationExecuted(_command));
                Emit(new OperationExecuted(_command));
            }
        }

        [Log]
        [Transactional]
        public class ExecuteOperation : ICommand
        {
            public bool AllowExecuteOperations { get; set; }

            public int OperationExecutedHandledCount { get; set; }
        }

        public class OperationExecuted : IDomainEvent
        {
            public ExecuteOperation Command { get; set; }

            public OperationExecuted(ExecuteOperation command)
            {
                Command = command;
            }
        }

        public class ExecuteOperationHandler : CommandHandler<ExecuteOperation>
        {
            public override void Handle(ExecuteOperation command)
            {
                var aggregate = new FakeAggregate(command);

                aggregate.Execute();

                Emit(aggregate.EmittedEvents);
            }
        }

        public class OperationExecutedEventHandler1 : IDomainEventHandler<OperationExecuted>
        {
            public void Handle(OperationExecuted domainEvent)
            {
                domainEvent.Command.OperationExecutedHandledCount++;
            }
        }

        public class OperationExecutedEventHandler2 : IDomainEventHandler<OperationExecuted>
        {
            public void Handle(OperationExecuted domainEvent)
            {
                domainEvent.Command.OperationExecutedHandledCount++;
            }
        }

        [Fact]
        public void AllowExecuteOperations_TheTwoEventsShouldBeHandledByTheTwoHandlers()
        {
            // Arrange

            var command = new ExecuteOperation {AllowExecuteOperations = true};

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.OperationExecutedHandledCount.Should().Be(4);
        }

        [Fact]
        public void DoNotAllowExecuteOperations_NoEventShouldBeHandled()
        {
            // Arrange

            var command = new ExecuteOperation();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.OperationExecutedHandledCount.Should().Be(0);
        }
    }
}