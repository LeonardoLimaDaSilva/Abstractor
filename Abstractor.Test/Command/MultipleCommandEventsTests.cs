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

                if (command.EventHandler1ThrowsException) throw new Exception();
            }
        }

        public class FakeEventHandler2 : IEventHandler<FakeCommandEvent>
        {
            public void Handle(FakeCommandEvent command)
            {
                command.EventHandler2Executed = true;

                if (command.EventHandler2ThrowsException) throw new Exception();
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
        public void SyncContext_CommandSucceeded_AllEventHandlersRegisteredForThisCommandShouldBeExecuted()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(
                () =>
                {
                    var command = new FakeCommandEvent();

                    // Act

                    CommandDispatcher.Dispatch(command);

                    // Assert

                    command.EventHandler1Executed.Should().Be.True();

                    command.EventHandler2Executed.Should().Be.True();
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);
        }

        [Fact]
        public void SyncContext_EventHandler1ThrowsException_EventHandler2ShouldBeExecutedIndependently()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(
                () =>
                {
                    var command = new FakeCommandEvent
                    {
                        EventHandler1ThrowsException = true
                    };

                    // Act

                    CommandDispatcher.Dispatch(command);

                    // Assert

                    command.EventHandler1Executed.Should().Be.True();

                    command.EventHandler2Executed.Should().Be.True();
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);
        }

        [Fact]
        public void SyncContext_EventHandler2ThrowsException_EventHandler1ShouldBeExecutedIndependently()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(
                () =>
                {
                    var command = new FakeCommandEvent
                    {
                        EventHandler2ThrowsException = true
                    };

                    // Act

                    CommandDispatcher.Dispatch(command);

                    // Assert

                    command.EventHandler1Executed.Should().Be.True();

                    command.EventHandler2Executed.Should().Be.True();
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);
        }
    }
}