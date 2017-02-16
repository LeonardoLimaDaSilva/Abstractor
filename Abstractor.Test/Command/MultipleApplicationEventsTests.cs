using System;
using System.Collections.Generic;
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
    ///     Tests for commands that has multiples event handlers subscribed to.
    /// </summary>
    public class MultipleApplicationEventsTests : BaseTest
    {
        /// <summary>
        ///     Command that is marked as an application event publisher.
        /// </summary>
        public class FakeCommand : ICommand, IApplicationEvent
        {
            public bool CommandThrowsException { get; set; }

            public bool EventHandler1Executed { get; set; }

            public bool EventHandler1Succeeded { get; set; }

            public bool EventHandler1ThrowsException { get; set; }

            public bool EventHandler2Executed { get; set; }

            public bool EventHandler2Succeeded { get; set; }

            public bool EventHandler2ThrowsException { get; set; }
        }

        /// <summary>
        ///     Handler of FakeCommand
        /// </summary>
        public class FakeCommandHandler : ICommandHandler<FakeCommand>
        {
            public IEnumerable<IDomainEvent> Handle(FakeCommand command)
            {
                if (command.CommandThrowsException) throw new Exception();
                yield break;
            }
        }

        /// <summary>
        ///     EventHandler1 that subscribes to FakeCommand.
        /// </summary>
        public class OnFakeCommandHandled1 : IApplicationEventHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
                command.EventHandler1Executed = true;

                if (command.EventHandler1ThrowsException) throw new Exception("Event 1 failed.");

                command.EventHandler1Succeeded = true;
            }
        }

        /// <summary>
        ///     EventHandler2 that subscribes to FakeCommand.
        /// </summary>
        public class OnFakeCommandHandled2 : IApplicationEventHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
                command.EventHandler2Executed = true;

                if (command.EventHandler2ThrowsException) throw new Exception("Event 2 failed.");

                command.EventHandler2Succeeded = true;
            }
        }

        [Fact]
        public void AsyncContext_CommandSucceeded_ShouldEnsureThatEvent1WasDispatchedAsyncly()
        {
            // Arrange

            var command = new FakeCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.EventHandler1Executed.Should().Be.False();
        }

        [Fact]
        public void AsyncContext_CommandSucceeded_ShouldEnsureThatEvent2WasDispatchedAsyncly()
        {
            // Arrange

            var command = new FakeCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.EventHandler2Executed.Should().Be.False();
        }

        [Fact]
        public void CommandThrowsException_NoneOfTheRegisteredEventHandlersShouldBeExecuted()
        {
            // Arrange

            var command = new FakeCommand
            {
                CommandThrowsException = true
            };

            // Act

            Assert.Throws<Exception>(() => CommandDispatcher.Dispatch(command));

            // Assert

            command.EventHandler1Executed.Should().Be.False();

            command.EventHandler2Executed.Should().Be.False();
        }

        [Fact]
        public void SyncContext_AllEventHandlersThrowsException_AllEventHandlersShouldBeExecutedIndependently()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var command = new FakeCommand
            {
                EventHandler1ThrowsException = true,
                EventHandler2ThrowsException = true
            };

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

            command.EventHandler1Executed.Should().Be.True();

            command.EventHandler1Succeeded.Should().Be.False();

            command.EventHandler2Executed.Should().Be.True();

            command.EventHandler2Succeeded.Should().Be.False();
        }

        [Fact]
        public void SyncContext_CommandSucceeded_AllEventHandlersRegisteredForThisCommandShouldBeExecuted()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var command = new FakeCommand();

            Task.Factory.StartNew(() =>
                    {
                        // Act

                        CommandDispatcher.Dispatch(command);
                    },
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    scheduler);

            // Assert

            command.EventHandler1Executed.Should().Be.True();

            command.EventHandler2Executed.Should().Be.True();
        }

        [Fact]
        public void SyncContext_EventHandler1ThrowsException_EventHandler2ShouldBeExecutedIndependently()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var command = new FakeCommand
            {
                EventHandler1ThrowsException = true
            };

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

            command.EventHandler1Executed.Should().Be.True();

            command.EventHandler1Succeeded.Should().Be.False();

            command.EventHandler2Executed.Should().Be.True();

            command.EventHandler2Succeeded.Should().Be.True();
        }
    }
}