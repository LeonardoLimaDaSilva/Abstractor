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
        }

        public class FakeCommandHandler : ICommandHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
                command.HandlerExecuted = true;
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
    }
}