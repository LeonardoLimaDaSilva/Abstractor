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
    public class MultipleCommandEventsTests : BaseTest
    {
        public class FakeCommandEvent : ICommandEvent
        {
            public bool CommandThrowsException { get; set; }

            public bool EventHandler1ThrowsException { get; set; }

            public bool EventHandler2ThrowsException { get; set; }

            public bool EventHandler1Executed { get; set; }

            public bool EventHandler2Executed { get; set; }

            public bool EventHandler1Succeeded { get; set; }

            public bool EventHandler2Succeeded { get; set; }
        }

        public class FakeCommandEventHandler : ICommandHandler<FakeCommandEvent>
        {
            public void Handle(FakeCommandEvent command)
            {
                if (command.CommandThrowsException) throw new Exception();
            }
        }

        public class FakeEventHandler1 : IEventHandler<FakeCommandEvent>
        {
            public void Handle(FakeCommandEvent command)
            {
                command.EventHandler1Executed = true;

                if (command.EventHandler1ThrowsException) throw new Exception("Event 1 failed.");

                command.EventHandler1Succeeded = true;
            }
        }

        public class FakeEventHandler2 : IEventHandler<FakeCommandEvent>
        {
            public void Handle(FakeCommandEvent command)
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

            var command = new FakeCommandEvent();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.EventHandler1Executed.Should().Be.False();
        }

        [Fact]
        public void AsyncContext_CommandSucceeded_ShouldEnsureThatEvent2WasDispatchedAsyncly()
        {
            // Arrange

            var command = new FakeCommandEvent();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.EventHandler2Executed.Should().Be.False();
        }

        [Fact]
        public void CommandThrowsException_NoneOfTheRegisteredEventHandlersShouldBeExecuted()
        {
            // Arrange

            var command = new FakeCommandEvent
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

            var command = new FakeCommandEvent
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

            var command = new FakeCommandEvent();

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

            command.EventHandler2Executed.Should().Be.True();
        }

        [Fact]
        public void SyncContext_EventHandler1ThrowsException_EventHandler2ShouldBeExecutedIndependently()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            var command = new FakeCommandEvent
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