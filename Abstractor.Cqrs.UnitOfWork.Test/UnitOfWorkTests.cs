using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.EntityFramework.Interfaces;
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
        public void Commit_EntityFrameworkThrowsException_ShouldLogExceptionRollbackAllAndRethrow(
            [Frozen]Mock<ILogger> logger,
            [Frozen]Mock<IEntityFrameworkContext> efContext,
            [Frozen]Mock<IAzureQueueContext> aqContext,
            [Frozen]Mock<IAzureTableContext> atContext,
            [Frozen]Mock<IAzureBlobContext> abContext,
            Persistence.UnitOfWork uow)
        {
            // Arrange

            var exception = new Exception("Exception");

            efContext.Setup(c => c.SaveChanges()).Throws(exception);

            // Act

            var assertException = Assert.Throws<Exception>(() => uow.Commit());

            assertException.Message.Should().Be("Exception");

            // Assert

            logger.Verify(l => l.Log(It.IsAny<string>()), Times.Exactly(7));

            abContext.Verify(c=>c.SaveChanges(),Times.Once);

            logger.Verify(l => l.Log("Azure Blob context commited."), Times.Once);

            atContext.Verify(c => c.SaveChanges(), Times.Once);

            logger.Verify(l => l.Log("Azure Table context commited."), Times.Once);

            aqContext.Verify(c => c.SaveChanges(), Times.Once);

            logger.Verify(l => l.Log("Azure Queue context commited."), Times.Once);

            logger.Verify(l => l.Log("Exception caught: Exception"), Times.Once);

            aqContext.Verify(c => c.Rollback(), Times.Once);

            logger.Verify(l => l.Log("Executing Azure Queue context rollback..."), Times.Once);

            atContext.Verify(c => c.Rollback(), Times.Once);

            logger.Verify(l => l.Log("Executing Azure Table context rollback..."), Times.Once);

            abContext.Verify(c => c.Rollback(), Times.Once);

            logger.Verify(l => l.Log("Executing Azure Blob context rollback..."), Times.Once);
        }
    }
}
