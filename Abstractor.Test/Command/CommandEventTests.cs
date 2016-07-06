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
    public class CommandEventTests : BaseTest
    {
        [Fact]
        public void Dispatch_WithinAsyncContext_CommandHandledSuccessfully_EventHandlerShouldBeExecuted()
        {
            // Arrange

            var scheduler = new SynchronousTaskScheduler();

            Task.Factory.StartNew(() =>
            {
                var command = new FakeCommandEvent();

                // Act

                CommandDispatcher.Dispatch(command);

                // Assert

                command.EventHandlerExecuted.Should().Be.True();
            },
                CancellationToken.None,
                TaskCreationOptions.None,
                scheduler);
        }

        [Fact]
        public void Dispatch_WithoutAsyncContext_CommandHandledSuccessfully_ShouldEnsureThatEventWasDispatchedAsyncly()
        {
            // Arrange

            var command = new FakeCommandEvent();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.EventHandlerExecuted.Should().Be.False();
        }

        [Fact]
        public void Dispatch_CommandThrowsException_EventHandlerShouldNotBeExecuted()
        {
            // Arrange

            var command = new FakeCommandEvent
            {
                CommandThrowsException = true
            };

            // Act

            Assert.Throws<Exception>(() => CommandDispatcher.Dispatch(command));

            // Assert

            command.EventHandlerExecuted.Should().Be.False();
        }

        public class FakeCommandEvent : ICommandEvent
        {
            public bool CommandThrowsException { get; set; }

            public bool EventHandlerExecuted { get; set; }
        }

        public class FakeCommandHandler : ICommandHandler<FakeCommandEvent>
        {
            public void Handle(FakeCommandEvent command)
            {
                if (command.CommandThrowsException) throw new Exception();
            }
        }

        public class FakeEventHandler : IEventHandler<FakeCommandEvent>
        {
            public void Handle(FakeCommandEvent command)
            {
                command.EventHandlerExecuted = true;
            }
        }
    }
}