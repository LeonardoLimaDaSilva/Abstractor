using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.Test.Helpers
{
    public class FakeDataSet<TEntity> : BaseDataSet<TEntity>
    {
        private TEntity _entity;

        public FakeDataSet(bool doRollback = true)
            : base(doRollback)
        {
        }

        public IList<BaseDataSetOperation> InternalOperations => Operations;

        public int Inserts { get; private set; }

        public int Deletes { get; private set; }

        public int Updates { get; private set; }

        public void SetUpGet(TEntity entity)
        {
            _entity = entity;
        }

        protected override void InsertEntity(TEntity entity)
        {
            Inserts++;
        }

        protected override void DeleteEntity(TEntity entity)
        {
            Deletes++;
        }

        protected override void UpdateEntity(TEntity entity)
        {
            Updates++;
        }

        protected override TEntity Get(TEntity entity)
        {
            return _entity;
        }
    }
}