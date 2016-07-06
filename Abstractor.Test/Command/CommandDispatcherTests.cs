using System.Threading;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Command
{
    public class CommandDispatcherTests : BaseTest
    {
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
        public void DispatchAsync_WithinAsyncContext_ShouldExecuteHandler()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(() =>
            {
                var command = new FakeCommand();

                // Act

                CommandDispatcher.DispatchAsync(command);

                // Assert

                command.HandlerExecuted.Should().Be.True();
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);
        }

        [Fact]
        public void DispatchAsync_WithoutSyncContext_ShouldEnsureThatCommandWasDispatchedAsyncly()
        {
            // Arrange

            var command = new FakeCommand();

            // Act

            CommandDispatcher.DispatchAsync(command);

            // Assert

            command.HandlerExecuted.Should().Be.False();
        }

        public class FakeCommand : ICommand
        {
            public bool HandlerExecuted { get; set; }
        }

        public class FakeCommandHandler : ICommandHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
                command.HandlerExecuted = true;
            }
        }
    }
}