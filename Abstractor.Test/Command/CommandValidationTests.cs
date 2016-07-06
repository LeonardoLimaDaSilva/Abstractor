using System.ComponentModel.DataAnnotations;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using Xunit;

namespace Abstractor.Test.Command
{
    public class CommandValidationTests : BaseTest
    {
        [Fact]
        public void ValidCommand_ShouldNotThrowException()
        {
            // Arrange

            var command = new ValidationCommand {Property = "Property"};

            // Act and assert

            CommandDispatcher.Dispatch(command);
        }

        [Fact]
        public void InvalidCommand_ShouldThrowValidationException()
        {
            // Arrange

            var command = new ValidationCommand();

            // Act and assert

            Assert.Throws<ValidationException>(() => CommandDispatcher.Dispatch(command));
        }

        public class ValidationCommand : ICommand
        {
            [Required]
            public string Property { get; set; }
        }

        public class ValidationCommandHandler : ICommandHandler<ValidationCommand>
        {
            public void Handle(ValidationCommand command)
            {
            }
        }
    }
}