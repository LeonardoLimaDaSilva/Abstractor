using System;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Test.Command
{
    public class CommandPostActionTests : BaseTest
    {
        public class PostActionCommand : ICommand
        {
            public bool ThrowException { get; set; }

            public bool ActionExecuted { get; set; }
        }

        public class PostActionCommandHandler : ICommandHandler<PostActionCommand>
        {
            private readonly ICommandPostAction _commandPostAction;

            public PostActionCommandHandler(ICommandPostAction commandPostAction)
            {
                _commandPostAction = commandPostAction;
            }

            public void Handle(PostActionCommand command)
            {
                _commandPostAction.Execute += () => { command.ActionExecuted = true; };

                if (command.ThrowException) throw new Exception();
            }
        }

        [Fact]
        public void Execute_CommandHandlerThrowsException_ActionShouldNotBeExecuted()
        {
            // Arrange

            var command = new PostActionCommand
            {
                ThrowException = true
            };

            // Act and assert

            Assert.Throws<Exception>(() => CommandDispatcher.Dispatch(command));

            command.ActionExecuted.Should().Be.False();
        }

        [Fact]
        public void Execute_CommandSucceeded_ActionShouldBeExecuted()
        {
            // Arrange

            var command = new PostActionCommand();

            // Act

            CommandDispatcher.Dispatch(command);

            // Assert

            command.ActionExecuted.Should().Be.True();
        }
    }
}