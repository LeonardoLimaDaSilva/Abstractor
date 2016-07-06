using System;
using System.Collections.Generic;
using System.Linq;

namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    public abstract class BaseDataSet<TEntity> : IBaseDataSet, IDisposable
    {
        private readonly bool _doRollback;

        protected IList<BaseDataSetOperation> Operations { get; }

        protected BaseDataSet(bool doRollback = true)
        {
            _doRollback = doRollback;
            Operations = new List<BaseDataSetOperation>();
        }

        public virtual void Commit()
        {
            Execute(false);
        }

        public virtual void Rollback()
        {
            if (_doRollback)
                Execute(true);
        }

        public virtual void Dispose()
        {
            foreach (var operation in Operations)
            {
                var d1 = operation.Entity as IDisposable;
                d1?.Dispose();

                d1 = operation.OldEntity as IDisposable;
                d1?.Dispose();
            }
        }

        private void Execute(bool done)
        {
            foreach (var operation in Operations.Where(p => p.Done == done).ToList())
            {
                switch (operation.Type)
                {
                    case BaseDataSetOperationType.Insert:
                        DoInsert(operation);
                        break;
                    case BaseDataSetOperationType.Delete:
                        DoDelete(operation);
                        break;
                    case BaseDataSetOperationType.Update:
                        DoUpdate(operation);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void DoUpdate(BaseDataSetOperation operation)
        {
            if (operation.OldEntity == null)
                operation.OldEntity = Get((TEntity) operation.Entity);

            UpdateEntity((TEntity) operation.Entity);

            operation.Done = !operation.Done;
            var updated = operation.OldEntity;
            operation.OldEntity = operation.Entity;
            operation.Entity = updated;
        }

        private void DoDelete(BaseDataSetOperation operation)
        {
            if (operation.OldEntity == null)
                operation.OldEntity = Get((TEntity) operation.Entity);

            DeleteEntity((TEntity) operation.Entity);

            operation.Done = !operation.Done;
            operation.Type = BaseDataSetOperationType.Insert;
            var deleted = operation.OldEntity;
            operation.OldEntity = operation.Entity;
            operation.Entity = deleted;
        }

        private void DoInsert(BaseDataSetOperation operation)
        {
            InsertEntity((TEntity) operation.Entity);

            operation.Done = !operation.Done;
            operation.Type = BaseDataSetOperationType.Delete;
        }

        protected abstract void InsertEntity(TEntity entity);
        protected abstract void DeleteEntity(TEntity entity);
        protected abstract void UpdateEntity(TEntity entity);
        protected abstract TEntity Get(TEntity entity);

        public void Insert(TEntity entity)
        {
            Operations.Add(new BaseDataSetOperation
            {
                Entity = entity, Type = BaseDataSetOperationType.Insert, Done = false
            });
        }

        public void Update(TEntity entity)
        {
            Operations.Add(new BaseDataSetOperation
            {
                Entity = entity, Type = BaseDataSetOperationType.Update, Done = false
            });
        }

        public void Delete(TEntity entity)
        {
            Operations.Add(new BaseDataSetOperation
            {
                Entity = entity, Type = BaseDataSetOperationType.Delete, Done = false
            });
        }
    }
}