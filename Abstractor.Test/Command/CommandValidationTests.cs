using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Test.Helpers;
using Xunit;

namespace Abstractor.Test.Command
{
    public class CommandValidationTests : BaseTest
    {
        public class ValidationCommand : ICommand
        {
            [Required]
            public string Property { get; set; }
        }

        public class ValidationCommandHandler : ICommandHandler<ValidationCommand>
        {
            public IEnumerable<IDomainEvent> Handle(ValidationCommand command)
            {
                yield break;
            }
        }

        [Fact]
        public void Validate_InvalidCommand_ShouldThrowValidationException()
        {
            // Arrange

            var command = new ValidationCommand();

            // Act and assert

            Assert.Throws<ValidationException>(() => CommandDispatcher.Dispatch(command));
        }

        [Fact]
        public void Validate_ValidCommand_ShouldNotThrowException()
        {
            // Arrange

            var command = new ValidationCommand {Property = "Property"};

            // Act and assert

            CommandDispatcher.Dispatch(command);
        }
    }
}