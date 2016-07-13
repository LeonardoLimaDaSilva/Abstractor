using System;
using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Interfaces.Persistence;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class CommandTransactionDecoratorTests
    {
        [Theory, AutoMoqData]
        public void Handle_Success_ShouldCommitTheUnitOfWorkAfterCommandHandled(
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            [Frozen] Mock<IUnitOfWork> unitOfWork,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            ICommand command,
            CommandTransactionDecorator<ICommand> decorator)
        {
            // Arrange and assert

            var callOrder = 0;

            commandHandler.Setup(d => d.Handle(command)).Callback(() => callOrder++.Should().Be(0));
            unitOfWork.Setup(d => d.Commit()).Callback(() => callOrder++.Should().Be(1));

            attributeFinder.Setup(f => f.Decorates(commandHandler.Object.GetType(), typeof (LogAttribute)))
                           .Returns(true);

            // Act

            decorator.Handle(command);

            // Assert

            logger.Verify(l => l.Log("Starting transaction..."), Times.Once);
            logger.Verify(l => l.Log("Transaction committed."), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Handle_WithoutLog_ShouldNotLog(
            [Frozen] Mock<ILogger> logger,
            ICommand command,
            CommandTransactionDecorator<ICommand> decorator)
        {
            // Act

            decorator.Handle(command);

            // Assert

            logger.Verify(l => l.Log("Starting transaction..."), Times.Never);
            logger.Verify(l => l.Log("Transaction committed."), Times.Never);
        }

        [Theory, AutoMoqData]
        public void Handle_Exception_ShouldNotCommitUnitOfWork(
            [Frozen] Mock<ICommandHandler<ICommand>> commandHandler,
            [Frozen] Mock<IUnitOfWork> unitOfWork,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IAttributeFinder> attributeFinder,
            ICommand command,
            CommandTransactionDecorator<ICommand> decorator)
        {
            // Arrange

            commandHandler.Setup(d => d.Handle(It.IsAny<ICommand>())).Throws<Exception>();

            attributeFinder.Setup(f => f.Decorates(commandHandler.Object.GetType(), typeof (LogAttribute)))
                           .Returns(true);

            // Act

            Assert.Throws<Exception>(() => decorator.Handle(command));

            // Assert

            unitOfWork.Verify(d => d.Commit(), Times.Never);

            logger.Verify(l => l.Log("Starting transaction..."), Times.Once);
            logger.Verify(l => l.Log("Transaction committed."), Times.Never);
        }
    }
}