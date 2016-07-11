using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Command
{
    /// <summary>
    ///     Tests for commands that has a single event handler subscribed to.
    /// </summary>
    public class SingleCommandEventTests : BaseTest
    {
        /// <summary>
        ///     Command that is marked as an event listener.
        /// </summary>
        public class FakeCommand : ICommand, IEventListener
        {
            public bool CommandThrowsException { get; set; }

            public bool EventHandlerExecuted { get; set; }
        }

        /// <summary>
        ///     Handler of FakeCommand
        /// </summary>
        public class FakeCommandHandler : ICommandHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
                if (command.CommandThrowsException) throw new Exception();
            }
        }

        /// <summary>
        ///     EventHandler1 that subscribes to FakeCommand.
        /// </summary>
        public class OnFakeCommandHandled : IEventHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
                command.EventHandlerExecuted = true;
            }
        }

        [Fact]
        public void AsyncContext_CommandSucceeded_ShouldEnsureThatEventWasDispatchedAsyncly()
        {
            // Arrange

            var command = new FakeCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.EventHandlerExecuted.Should().Be.False();
        }

        [Fact]
        public void CommandThrowsException_EventHandlerShouldNotBeExecuted()
        {
            // Arrange

            var command = new FakeCommand
            {
                CommandThrowsException = true
            };

            // Act

            Assert.Throws<Exception>(() => CommandDispatcher.Dispatch(command));

            // Assert

            command.EventHandlerExecuted.Should().Be.False();
        }

        [Fact]
        public void SyncContext_CommandSucceeded_EventHandlerShouldBeExecuted()
        {
            // Arrange

            Logger.SetUp();

            var scheduler = new SynchronousTaskScheduler();

            var command = new FakeCommand();

            Task.Factory.StartNew(
                () =>
                {
                    // Act

                    CommandDispatcher.Dispatch(command);
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);

            // Assert

            command.EventHandlerExecuted.Should().Be.True();

            Logger.MessagesShouldBe(
                "Executing command \"FakeCommand\" with the parameters:",
                "{\r\n  \"CommandThrowsException\": false,\r\n  \"EventHandlerExecuted\": false\r\n}",
                "Executing event \"OnFakeCommandHandled\" with the listener parameters:",
                "{\r\n  \"CommandThrowsException\": false,\r\n  \"EventHandlerExecuted\": false\r\n}",
                "Event \"OnFakeCommandHandled\" executed in 00:00:00.",
                "Command \"FakeCommand\" executed in 00:00:00.");
        }
    }
}