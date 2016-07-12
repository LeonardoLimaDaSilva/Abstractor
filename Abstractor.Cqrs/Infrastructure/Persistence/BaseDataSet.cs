using System;
using System.Collections.Generic;
using System.Linq;

namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     Base data set that stores the write and read operations and uses template method to abstract the commit and
    ///     rollback algorithm.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public abstract class BaseDataSet<TEntity> : IBaseDataSet, IDisposable
    {
        private readonly bool _doRollback;

        protected IList<BaseDataSetOperation> Operations { get; }

        /// <param name="doRollback">Optional parameter that informs whether the data set can rollback the operations.</param>
        protected BaseDataSet(bool doRollback = true)
        {
            _doRollback = doRollback;
            Operations = new List<BaseDataSetOperation>();
        }

        /// <summary>
        ///     Commits all pending operations.
        /// </summary>
        public virtual void Commit()
        {
            Execute(false);
        }

        /// <summary>
        ///     Undoes all done operations.
        /// </summary>
        public virtual void Rollback()
        {
            if (_doRollback)
                Execute(true);
        }

        /// <summary>
        ///     Disposes all the disposable entities stored in the operations.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var operation in Operations)
            {
                var d1 = operation.CurrentEntity as IDisposable;
                d1?.Dispose();

                d1 = operation.PreviousEntity as IDisposable;
                d1?.Dispose();
            }
        }

        /// <summary>
        ///     Executes all pending operations.
        /// </summary>
        /// <param name="done">State to filter the operations.</param>
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
                }
            }
        }

        /// <summary>
        ///     Stores the previous state and calls the update hook.
        /// </summary>
        /// <param name="operation">Operation to be performed.</param>
        private void DoUpdate(BaseDataSetOperation operation)
        {
            if (operation.PreviousEntity == null)
                operation.PreviousEntity = Get((TEntity) operation.CurrentEntity);

            UpdateEntity((TEntity) operation.CurrentEntity);

            operation.Done = !operation.Done;
            var updated = operation.PreviousEntity;
            operation.PreviousEntity = operation.CurrentEntity;
            operation.CurrentEntity = updated;
        }

        /// <summary>
        ///     Stores the previous state, calls the delete hook and sets the insert operation.
        /// </summary>
        /// <param name="operation">Operation to be performed.</param>
        private void DoDelete(BaseDataSetOperation operation)
        {
            if (operation.PreviousEntity == null)
                operation.PreviousEntity = Get((TEntity) operation.CurrentEntity);

            DeleteEntity((TEntity) operation.CurrentEntity);

            operation.Done = !operation.Done;
            operation.Type = BaseDataSetOperationType.Insert;
            var deleted = operation.PreviousEntity;
            operation.PreviousEntity = operation.CurrentEntity;
            operation.CurrentEntity = deleted;
        }

        /// <summary>
        ///     Calls the insert hook and sets the delete operation.
        /// </summary>
        /// <param name="operation">Operation to be performed.</param>
        private void DoInsert(BaseDataSetOperation operation)
        {
            InsertEntity((TEntity) operation.CurrentEntity);

            operation.Done = !operation.Done;
            operation.Type = BaseDataSetOperationType.Delete;
        }

        /// <summary>
        ///     Hook to the actual insert method of underlying persistence mechanism.
        /// </summary>
        /// <param name="entity">Entity type</param>
        protected abstract void InsertEntity(TEntity entity);

        /// <summary>
        ///     Hook to the actual delete method of underlying persistence mechanism.
        /// </summary>
        /// <param name="entity">Entity type</param>
        protected abstract void DeleteEntity(TEntity entity);

        /// <summary>
        ///     Hook to the actual update method of underlying persistence mechanism.
        /// </summary>
        /// <param name="entity">Entity type</param>
        protected abstract void UpdateEntity(TEntity entity);

        /// <summary>
        ///     Hook to the actual get method of underlying persistence mechanism.
        /// </summary>
        /// <param name="entity">Entity type</param>
        protected abstract TEntity Get(TEntity entity);

        /// <summary>
        ///     Adds an insert operation.
        /// </summary>
        /// <param name="entity">Entity type.</param>
        public void Insert(TEntity entity)
        {
            Operations.Add(new BaseDataSetOperation
            {
                CurrentEntity = entity,
                Type = BaseDataSetOperationType.Insert,
                Done = false
            });
        }

        /// <summary>
        ///     Adds an update operation.
        /// </summary>
        /// <param name="entity">Entity type.</param>
        public void Update(TEntity entity)
        {
            Operations.Add(new BaseDataSetOperation
            {
                CurrentEntity = entity,
                Type = BaseDataSetOperationType.Update,
                Done = false
            });
        }

        /// <summary>
        ///     Adds a delete operation.
        /// </summary>
        /// <param name="entity">Entity type.</param>
        public void Delete(TEntity entity)
        {
            Operations.Add(new BaseDataSetOperation
            {
                CurrentEntity = entity,
                Type = BaseDataSetOperationType.Delete,
                Done = false
            });
        }
    }
}