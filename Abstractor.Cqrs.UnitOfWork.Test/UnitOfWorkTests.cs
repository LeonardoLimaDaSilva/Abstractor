using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.EntityFramework.Extensions;
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
        [Theory]
        [AutoMoqData]
        public void Clear_ShouldClearAllAndLog(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEntityFrameworkContext> efContext,
            [Frozen] Mock<IAzureQueueContext> aqContext,
            [Frozen] Mock<IAzureTableContext> atContext,
            [Frozen] Mock<IAzureBlobContext> abContext,
            [Frozen] Mock<IEntityFrameworkChangeTracker> changeTracker,
            Persistence.UnitOfWork uow)
        {
            EntityFrameworkContextExtensions.Factory = efc => changeTracker.Object;

            // Act

            uow.Clear();

            // Assert

            changeTracker.Verify(t => t.Clear(), Times.Once);
            aqContext.Verify(c => c.Clear(), Times.Once);
            atContext.Verify(c => c.Clear(), Times.Once);
            abContext.Verify(c => c.Clear(), Times.Once);

            logger.Verify(l => l.Log("Clearing Entity Framework context..."), Times.Once);
            logger.Verify(l => l.Log("Clearing Azure Queue context..."), Times.Once);
            logger.Verify(l => l.Log("Clearing Azure Table context..."), Times.Once);
            logger.Verify(l => l.Log("Clearing Azure Blob context..."), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Commit_AzureBlobThrowsException_ShouldRollbackAllAndLog(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEntityFrameworkContext> efContext,
            [Frozen] Mock<IAzureQueueContext> aqContext,
            [Frozen] Mock<IAzureTableContext> atContext,
            [Frozen] Mock<IAzureBlobContext> abContext,
            Persistence.UnitOfWork uow)
        {
            // Arrange

            var exception = new Exception("Exception");

            abContext.Setup(c => c.SaveChanges()).Throws(exception);

            // Act

            var assertException = Assert.Throws<Exception>(() => uow.Commit());

            assertException.Message.Should().Be("Exception");

            // Assert

            logger.Verify(l => l.Log("Exception caught: Exception"), Times.Once);

            abContext.Verify(c => c.SaveChanges(), Times.Once);
            atContext.Verify(c => c.SaveChanges(), Times.Never);
            aqContext.Verify(c => c.SaveChanges(), Times.Never);
            efContext.Verify(c => c.SaveChanges(), Times.Never);

            logger.Verify(l => l.Log("Azure Blob context commited."), Times.Never);
            logger.Verify(l => l.Log("Azure Table context commited."), Times.Never);
            logger.Verify(l => l.Log("Azure Queue context commited."), Times.Never);
            logger.Verify(l => l.Log("Entity Framework context commited."), Times.Never);

            aqContext.Verify(c => c.Rollback(), Times.Once);
            atContext.Verify(c => c.Rollback(), Times.Once);
            abContext.Verify(c => c.Rollback(), Times.Once);

            logger.Verify(l => l.Log("Executing Azure Queue context rollback..."), Times.Once);
            logger.Verify(l => l.Log("Executing Azure Table context rollback..."), Times.Once);
            logger.Verify(l => l.Log("Executing Azure Blob context rollback..."), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Commit_AzureQueueThrowsException_ShouldRollbackAllAndLog(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEntityFrameworkContext> efContext,
            [Frozen] Mock<IAzureQueueContext> aqContext,
            [Frozen] Mock<IAzureTableContext> atContext,
            [Frozen] Mock<IAzureBlobContext> abContext,
            Persistence.UnitOfWork uow)
        {
            // Arrange

            var exception = new Exception("Exception");

            aqContext.Setup(c => c.SaveChanges()).Throws(exception);

            // Act

            var assertException = Assert.Throws<Exception>(() => uow.Commit());

            assertException.Message.Should().Be("Exception");

            // Assert

            logger.Verify(l => l.Log("Exception caught: Exception"), Times.Once);

            abContext.Verify(c => c.SaveChanges(), Times.Once);
            atContext.Verify(c => c.SaveChanges(), Times.Once);
            aqContext.Verify(c => c.SaveChanges(), Times.Once);
            efContext.Verify(c => c.SaveChanges(), Times.Never);

            logger.Verify(l => l.Log("Azure Blob context commited."), Times.Once);
            logger.Verify(l => l.Log("Azure Table context commited."), Times.Once);
            logger.Verify(l => l.Log("Azure Queue context commited."), Times.Never);
            logger.Verify(l => l.Log("Entity Framework context commited."), Times.Never);

            aqContext.Verify(c => c.Rollback(), Times.Once);
            atContext.Verify(c => c.Rollback(), Times.Once);
            abContext.Verify(c => c.Rollback(), Times.Once);

            logger.Verify(l => l.Log("Executing Azure Queue context rollback..."), Times.Once);
            logger.Verify(l => l.Log("Executing Azure Table context rollback..."), Times.Once);
            logger.Verify(l => l.Log("Executing Azure Blob context rollback..."), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Commit_AzureTableThrowsException_ShouldRollbackAllAndLog(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEntityFrameworkContext> efContext,
            [Frozen] Mock<IAzureQueueContext> aqContext,
            [Frozen] Mock<IAzureTableContext> atContext,
            [Frozen] Mock<IAzureBlobContext> abContext,
            Persistence.UnitOfWork uow)
        {
            // Arrange

            var exception = new Exception("Exception");

            atContext.Setup(c => c.SaveChanges()).Throws(exception);

            // Act

            var assertException = Assert.Throws<Exception>(() => uow.Commit());

            assertException.Message.Should().Be("Exception");

            // Assert

            logger.Verify(l => l.Log("Exception caught: Exception"), Times.Once);

            abContext.Verify(c => c.SaveChanges(), Times.Once);
            atContext.Verify(c => c.SaveChanges(), Times.Once);
            aqContext.Verify(c => c.SaveChanges(), Times.Never);
            efContext.Verify(c => c.SaveChanges(), Times.Never);

            logger.Verify(l => l.Log("Azure Blob context commited."), Times.Once);
            logger.Verify(l => l.Log("Azure Table context commited."), Times.Never);
            logger.Verify(l => l.Log("Azure Queue context commited."), Times.Never);
            logger.Verify(l => l.Log("Entity Framework context commited."), Times.Never);

            aqContext.Verify(c => c.Rollback(), Times.Once);
            atContext.Verify(c => c.Rollback(), Times.Once);
            abContext.Verify(c => c.Rollback(), Times.Once);

            logger.Verify(l => l.Log("Executing Azure Queue context rollback..."), Times.Once);
            logger.Verify(l => l.Log("Executing Azure Table context rollback..."), Times.Once);
            logger.Verify(l => l.Log("Executing Azure Blob context rollback..."), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Commit_EntityFrameworkThrowsException_ShouldRollbackAllAndLog(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEntityFrameworkContext> efContext,
            [Frozen] Mock<IAzureQueueContext> aqContext,
            [Frozen] Mock<IAzureTableContext> atContext,
            [Frozen] Mock<IAzureBlobContext> abContext,
            Persistence.UnitOfWork uow)
        {
            // Arrange

            var exception = new Exception("Exception");

            efContext.Setup(c => c.SaveChanges()).Throws(exception);

            // Act

            var assertException = Assert.Throws<Exception>(() => uow.Commit());

            assertException.Message.Should().Be("Exception");

            // Assert

            logger.Verify(l => l.Log("Exception caught: Exception"), Times.Once);

            abContext.Verify(c => c.SaveChanges(), Times.Once);
            atContext.Verify(c => c.SaveChanges(), Times.Once);
            aqContext.Verify(c => c.SaveChanges(), Times.Once);
            efContext.Verify(c => c.SaveChanges(), Times.Once);

            logger.Verify(l => l.Log("Azure Blob context commited."), Times.Once);
            logger.Verify(l => l.Log("Azure Table context commited."), Times.Once);
            logger.Verify(l => l.Log("Azure Queue context commited."), Times.Once);
            logger.Verify(l => l.Log("Entity Framework context commited."), Times.Never);

            aqContext.Verify(c => c.Rollback(), Times.Once);
            atContext.Verify(c => c.Rollback(), Times.Once);
            abContext.Verify(c => c.Rollback(), Times.Once);

            logger.Verify(l => l.Log("Executing Azure Queue context rollback..."), Times.Once);
            logger.Verify(l => l.Log("Executing Azure Table context rollback..."), Times.Once);
            logger.Verify(l => l.Log("Executing Azure Blob context rollback..."), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Commit_Success_ShouldCommitAllAndLog(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IEntityFrameworkContext> efContext,
            [Frozen] Mock<IAzureQueueContext> aqContext,
            [Frozen] Mock<IAzureTableContext> atContext,
            [Frozen] Mock<IAzureBlobContext> abContext,
            Persistence.UnitOfWork uow)
        {
            // Arrange

            // Act

            uow.Commit();

            // Assert

            efContext.Verify(c => c.SaveChanges(), Times.Once);
            aqContext.Verify(c => c.SaveChanges(), Times.Once);
            atContext.Verify(c => c.SaveChanges(), Times.Once);
            abContext.Verify(c => c.SaveChanges(), Times.Once);

            logger.Verify(l => l.Log("Azure Blob context commited."), Times.Once);
            logger.Verify(l => l.Log("Azure Table context commited."), Times.Once);
            logger.Verify(l => l.Log("Azure Queue context commited."), Times.Once);
            logger.Verify(l => l.Log("Entity Framework context commited."), Times.Once);
        }
    }
}