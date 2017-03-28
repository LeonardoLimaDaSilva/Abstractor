using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Operations;
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
    public class SingleApplicationEventTests : BaseTest
    {
        /// <summary>
        ///     Command that is marked as an event publisher.
        /// </summary>
        public class FakeCommand : ICommand, IApplicationEvent
        {
            public bool CommandThrowsGenericException { get; set; }

            public bool CommandThrowsSpecificException { get; set; }

            public bool EventHandlerExecuted { get; set; }

            public bool SpecificExceptionHandlerExecuted { get; set; }
        }

        /// <summary>
        ///     Handler of FakeCommand
        /// </summary>
        public class FakeCommandHandler : CommandHandler<FakeCommand>
        {
            public override void Handle(FakeCommand command)
            {
                if (command.CommandThrowsGenericException) throw new Exception();
                if (command.CommandThrowsSpecificException) throw new SpecificException(command);
            }
        }

        /// <summary>
        ///     Event handler that subscribes to FakeCommand.
        /// </summary>
        public class OnFakeCommandHandled : IApplicationEventHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
                command.EventHandlerExecuted = true;
            }
        }

        /// <summary>
        ///     The specific command exception is an event listener too.
        /// </summary>
        public class SpecificException : CommandException
        {
            public FakeCommand Command { get; }

            public SpecificException(FakeCommand command)
            {
                Command = command;
            }
        }

        /// <summary>
        ///     Event handler that subscribes to SpecificException.
        /// </summary>
        public class OnSpecificException : IApplicationEventHandler<SpecificException>
        {
            public void Handle(SpecificException eventListener)
            {
                eventListener.Command.SpecificExceptionHandlerExecuted = true;
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
        public void CommandThrowsGenericException_EventHandlerShouldNotBeExecutedAndExceptionRethrows()
        {
            // Arrange

            var command = new FakeCommand
            {
                CommandThrowsGenericException = true
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
        }

        [Fact]
        public void
            SyncContext_CommandThrowsSpecificException_EventHandlerShouldNotBeExecuted_ShouldHandleExceptionAndRethrow()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var command = new FakeCommand
            {
                CommandThrowsSpecificException = true
            };

            // Act

            Task.Factory.StartNew(() =>
                    {
                        // Act

                        Assert.Throws<SpecificException>(() => CommandDispatcher.Dispatch(command));
                    },
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    scheduler);

            // Assert

            command.EventHandlerExecuted.Should().Be.False();

            command.SpecificExceptionHandlerExecuted.Should().Be.True();
        }
    }
}