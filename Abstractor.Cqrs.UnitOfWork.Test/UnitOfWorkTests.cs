using System;
using System.Data.Entity;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.UnitOfWork.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.UnitOfWork.Test
{
    public class UnitOfWorkTests
    {
        [Theory, AutoMoqData]
        public void Commit_Success_ShouldLogTheSuccess(
            [Frozen]Mock<ILogger> logger,
            Persistence.UnitOfWork uow)
        {
            // Arrange

            // Act

            uow.Commit();

            // Assert

            logger.Verify(l => l.Log(It.IsAny<string>()), Times.Exactly(4));

            logger.Verify(l => l.Log("Azure Blob context commited."), Times.Once);

            logger.Verify(l => l.Log("Azure Table context commited."), Times.Once);

            logger.Verify(l => l.Log("Azure Queue context commited."), Times.Once);

            logger.Verify(l => l.Log("Entity Framework context commited."), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Commit_DbContextThrowsException_ShouldLogExceptionAndRethrow(
            [Frozen]Mock<ILogger> logger,
            [Frozen]Mock<DbContext> dbContext,
            Persistence.UnitOfWork uow)
        {
            // Arrange

            var exception = new Exception("Exception");

            dbContext.Setup(c => c.SaveChanges()).Throws(exception);

            // Act

            var assertException = Assert.Throws<Exception>(() => uow.Commit());

            assertException.Message.Should().Be("Exception");

            // Assert

            logger.Verify(l => l.Log(It.IsAny<string>()), Times.Once);
            
            logger.Verify(l => l.Log("Exception caught: Exception"), Times.Once);
        }
    }
}
