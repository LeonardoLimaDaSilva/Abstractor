using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.Test.Helpers
{
    public class FakeDataContext : BaseDataContext
    {
        private object _dataSet;

        public void SetUpDataSet<TEntity>(BaseDataSet<TEntity> dataSet)
        {
            _dataSet = dataSet;
        }

        protected override IBaseDataSet GetDataSet<TEntity>()
        {
            return (IBaseDataSet) _dataSet;
        }
    }
}