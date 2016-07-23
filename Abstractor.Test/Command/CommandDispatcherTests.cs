using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Command
{
    public class CommandDispatcherTests : BaseTest
    {
        public class FakeCommand : ICommand
        {
            public bool HandlerExecuted { get; set; }

            public bool ThrowCommandException { get; set; }

            public bool ExceptionHandlerExecuted { get; set; }
        }

        public class FakeCommandHandler : ICommandHandler<FakeCommand>
        {
            public IEnumerable<IDomainEvent> Handle(FakeCommand command)
            {
                command.HandlerExecuted = true;

                if (command.ThrowCommandException)
                    throw new FakeCommandException(command);

                yield break;
            }
        }

        public class FakeCommandException : CommandException
        {
            public FakeCommand FakeCommand { get; }

            public FakeCommandException(FakeCommand fakeCommand)
            {
                FakeCommand = fakeCommand;
            }
        }

        public class FakeCommandExceptionHandler : ICommandHandler<FakeCommandException>
        {
            public IEnumerable<IDomainEvent> Handle(FakeCommandException command)
            {
                command.FakeCommand.ExceptionHandlerExecuted = true;
                yield break;
            }
        }

        [Fact]
        public void Dispatch_ShouldExecuteHandler()
        {
            // Arrange

            var command = new FakeCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.HandlerExecuted.Should().Be.True();
        }

        [Fact]
        public void DispatchAsync_DoNotAwait_ShouldEnsureThatCommandWasDispatchedAsyncly()
        {
            // Arrange

            var command = new FakeCommand();

            // Act

            CommandDispatcher.DispatchAsync(command);

            // Assert

            command.HandlerExecuted.Should().Be.False();
        }

        [Fact]
        public async void DispatchAsync_Await_ShouldExecuteHandler()
        {
            // Arrange

            var command = new FakeCommand();

            await CommandDispatcher.DispatchAsync(command);

            // Assert

            command.HandlerExecuted.Should().Be.True();
        }

        [Fact]
        public void Dispatch_ThrowsCommandException_ShouldExecuteExceptionHandlerAndRethrow()
        {
            // Arrange

            var command = new FakeCommand {ThrowCommandException = true};

            // Act

            Assert.Throws<FakeCommandException>(() => CommandDispatcher.Dispatch(command));

            // Assert

            command.HandlerExecuted.Should().Be.True();

            command.ExceptionHandlerExecuted.Should().Be.True();
        }

        [Fact]
        public async void DispatchAsync_ThrowsCommandException_ShouldExecuteExceptionHandlerAndRethrow()
        {
            // Arrange

            var command = new FakeCommand { ThrowCommandException = true };

            // Act

            await Assert.ThrowsAsync<FakeCommandException>(() => CommandDispatcher.DispatchAsync(command));

            // Assert

            command.HandlerExecuted.Should().Be.True();

            command.ExceptionHandlerExecuted.Should().Be.True();
        }
    }
}