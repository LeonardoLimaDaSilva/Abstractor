using System;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class CommandValidationDecoratorTests
    {
        [Theory]
        [AutoMoqData]
        public void Handle_CommandInvalid_ShouldThrowExceptionAndNotHandleCommand(
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            [Frozen] Mock<IValidator> validator,
            ICommand command,
            CommandValidationDecorator<ICommand> decorator)
        {
            // Arrange

            validator.Setup(d => d.Validate(It.IsAny<ICommand>())).Throws<Exception>();

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            commandHandler.Verify(d => d.Handle(It.IsAny<ICommand>()), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public void Handle_CommandValid_ShouldHandleCommandAfterValidation(
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            [Frozen] Mock<IValidator> validator,
            ICommand command,
            CommandValidationDecorator<ICommand> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            validator.Setup(d => d.Validate(command)).Callback(() => callOrder++.Should().Be(0));
            commandHandler.Setup(d => d.Handle(command)).Callback(() => callOrder++.Should().Be(1));

            // Act

            decorator.Handle(command);
        }
    }
}