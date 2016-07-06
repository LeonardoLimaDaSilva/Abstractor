using Abstractor.Cqrs.Infrastructure.Persistence;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Persistence
{
    public class BaseDataContextTests
    {
        [Theory, AutoMoqData]
        public void Set_DataSetAlreadyExistsInInternalContext_ShouldReturnSameInstance(
            FakeDataContext context)
        {
            // Act

            var dataSet1 = context.Set<object>();
            var dataSet2 = context.Set<object>();

            // Assert

            dataSet1.Should().Be.EqualTo(dataSet2);
        }

        [Theory, AutoMoqData]
        public void SaveChanges_MultipleDataSets_ShouldCommitAllInternalContexts(
            Mock<BaseDataSet<int>> dataSet1,
            Mock<BaseDataSet<string>> dataSet2,
            FakeDataContext context)
        {
            // Arrange

            context.SetUpDataSet(dataSet1.Object);
            context.Set<int>();

            context.SetUpDataSet(dataSet2.Object);
            context.Set<string>();

            // Act

            context.SaveChanges();

            // Assert

            dataSet1.Verify(s => s.Commit(), Times.Once);
            dataSet2.Verify(s => s.Commit(), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Rollback_MultipleDataSets_ShouldRollbackAllInternalContexts(
            Mock<BaseDataSet<int>> dataSet1,
            Mock<BaseDataSet<string>> dataSet2,
            FakeDataContext context)
        {
            // Arrange

            context.SetUpDataSet(dataSet1.Object);
            context.Set<int>();

            context.SetUpDataSet(dataSet2.Object);
            context.Set<string>();

            // Act

            context.Rollback();

            // Assert

            dataSet1.Verify(s => s.Rollback(), Times.Once);
            dataSet2.Verify(s => s.Rollback(), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Dispose_MultipleDataSets_ShouldDisposeAllInternalContexts(
            Mock<BaseDataSet<int>> dataSet1,
            Mock<BaseDataSet<string>> dataSet2,
            FakeDataContext context)
        {
            // Arrange

            context.SetUpDataSet(dataSet1.Object);
            context.Set<int>();

            context.SetUpDataSet(dataSet2.Object);
            context.Set<string>();

            // Act

            context.Dispose();

            // Assert

            dataSet1.Verify(s => s.Dispose(), Times.Once);
            dataSet2.Verify(s => s.Dispose(), Times.Once);
        }
    }
}