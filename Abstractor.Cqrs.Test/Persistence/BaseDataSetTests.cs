using System;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Persistence
{
    public class BaseDataSetTests
    {
        [Theory, AutoMoqData]
        internal void Insert_WithoutCommit_ShouldCreateANewInsertOperation(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Insert(entity);

            // Assert

            var operation = dataSet.InternalOperations.Single();

            operation.Entity.Should().Be.EqualTo(entity);
            operation.Type.Should().Be(BaseDataSetOperationType.Insert);
            operation.Done.Should().Be.False();
        }

        [Theory, AutoMoqData]
        internal void Insert_WithCommit_ShouldChangeOperationTypeToDeleteAndSetAsDone(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Insert(entity);
            dataSet.Commit();

            // Assert

            var operation = dataSet.InternalOperations.Single();

            operation.Type.Should().Be(BaseDataSetOperationType.Delete);
            operation.Done.Should().Be.True();
        }

        [Theory, AutoMoqData]
        internal void Insert_WithCommitAndRollback_ShouldInsertAfterCommitAndDeleteAfterRollback(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Insert(entity);
            dataSet.Commit();

            // Assert

            dataSet.Inserts.Should().Be(1);
            dataSet.Deletes.Should().Be(0);
            dataSet.Updates.Should().Be(0);

            dataSet.Rollback();

            dataSet.Inserts.Should().Be(1);
            dataSet.Deletes.Should().Be(1);
            dataSet.Updates.Should().Be(0);
        }

        [Theory, AutoMoqData]
        internal void Delete_WithoutCommit_ShouldCreateANewDeleteOperation(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Delete(entity);

            // Assert

            var operation = dataSet.InternalOperations.Single();

            operation.Entity.Should().Be.EqualTo(entity);
            operation.Type.Should().Be(BaseDataSetOperationType.Delete);
            operation.Done.Should().Be.False();
        }

        [Theory, AutoMoqData]
        internal void Delete_WithCommit_ShouldChangeOperationTypeToInsertAndSetAsDone(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Delete(entity);
            dataSet.Commit();

            // Assert

            var operation = dataSet.InternalOperations.Single();

            operation.Type.Should().Be(BaseDataSetOperationType.Insert);
            operation.Done.Should().Be.True();
        }

        [Theory, AutoMoqData]
        internal void Delete_WithCommitAndRollback_ShouldDeleteAfterCommitAndInsertAfterRollback(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Delete(entity);
            dataSet.Commit();

            // Assert

            dataSet.Inserts.Should().Be(0);
            dataSet.Deletes.Should().Be(1);
            dataSet.Updates.Should().Be(0);

            dataSet.Rollback();

            dataSet.Inserts.Should().Be(1);
            dataSet.Deletes.Should().Be(1);
            dataSet.Updates.Should().Be(0);
        }

        [Theory, AutoMoqData]
        internal void Update_WithoutCommit_ShouldCreateANewUpdateOperation(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Update(entity);

            // Assert

            var operation = dataSet.InternalOperations.Single();

            operation.Entity.Should().Be.EqualTo(entity);
            operation.Type.Should().Be(BaseDataSetOperationType.Update);
            operation.Done.Should().Be.False();
        }

        [Theory, AutoMoqData]
        internal void Update_WithCommit_ShouldSetTheOriginalEntityToOldEntityAndSetAsDone(
            object entity1,
            object entity2,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.SetUpGet(entity1);

            dataSet.Update(entity2);
            dataSet.Commit();

            // Assert

            var operation = dataSet.InternalOperations.Single();

            operation.Type.Should().Be(BaseDataSetOperationType.Update);
            operation.Entity.Should().Not.Be.Null();
            operation.Entity.Should().Be.EqualTo(entity1);
            operation.OldEntity.Should().Be.EqualTo(entity2);
            operation.Done.Should().Be.True();
        }

        [Theory, AutoMoqData]
        internal void Update_WithCommitAndRollback_ShouldCallUpdateTwice(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Update(entity);
            dataSet.Commit();

            // Assert

            dataSet.Inserts.Should().Be(0);
            dataSet.Deletes.Should().Be(0);
            dataSet.Updates.Should().Be(1);

            dataSet.Rollback();

            dataSet.Inserts.Should().Be(0);
            dataSet.Deletes.Should().Be(0);
            dataSet.Updates.Should().Be(2);

            var operation = dataSet.InternalOperations.Single();

            operation.Entity.Should().Not.Be.EqualTo(operation.OldEntity);
        }

        [Theory, AutoMoqData]
        internal void Rollback_WithoutCommit_ShouldNotExecuteAnyOperations(
            object entity,
            FakeDataSet<object> dataSet)
        {
            // Act

            dataSet.Insert(entity);
            dataSet.Delete(entity);
            dataSet.Update(entity);

            dataSet.Rollback();

            // Assert

            dataSet.Inserts.Should().Be(0);
            dataSet.Deletes.Should().Be(0);
            dataSet.Updates.Should().Be(0);
        }

        [Theory, AutoMoqData]
        internal void Rollback_PassingDoRollbackFalseInConstructor_ShouldNotRollbackTheOperations(
            object entity)
        {
            // Arrange

            var dataSet = new FakeDataSet<object>(false);

            // Act

            dataSet.Insert(entity);
            dataSet.Delete(entity);
            dataSet.Update(entity);

            dataSet.Commit();
            dataSet.Rollback();

            // Assert

            dataSet.Inserts.Should().Be(1);
            dataSet.Deletes.Should().Be(1);
            dataSet.Updates.Should().Be(1);
        }

        [Theory, AutoMoqData]
        internal void Dispose_ShouldDisposeTheEntitiesStoredOnOperations(
            Mock<IDisposable> entity1,
            Mock<IDisposable> entity2,
            FakeDataSet<object> dataSet)
        {
            // Arrange

            dataSet.SetUpGet(entity1.Object);

            dataSet.Update(entity2.Object);
            dataSet.Commit();

            // Act

            dataSet.Dispose();

            // Assert

            entity1.Verify(e => e.Dispose(), Times.Once);
            entity2.Verify(e => e.Dispose(), Times.Once);
        }
    }
}