using System;
using System.Collections.Generic;
using System.Linq;

namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     Base data context responsible for create the entities data sets and commit and rollback the internal data set
    ///     operations.
    /// </summary>
    public abstract class BaseDataContext : IDisposable
    {
        /// <summary>
        ///     Stores the data sets created for each entity type.
        /// </summary>
        private readonly IDictionary<string, IBaseDataSet> _internalContext;

        protected BaseDataContext()
        {
            _internalContext = new Dictionary<string, IBaseDataSet>();
        }

        /// <summary>
        ///     Disposes all the stored data sets.
        /// </summary>
        public void Dispose()
        {
            foreach (var d in _internalContext.Values.Select(dataSet => dataSet as IDisposable))
                d?.Dispose();
        }

        /// <summary>
        ///     Creates a new data set for the entity type.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>Data set of a specific entity type.</returns>
        public BaseDataSet<TEntity> Set<TEntity>()
        {
            if (_internalContext.ContainsKey(typeof (TEntity).Name))
                return (BaseDataSet<TEntity>) _internalContext[typeof (TEntity).Name];

            var set = GetDataSet<TEntity>();

            _internalContext.Add(typeof (TEntity).Name, set);

            return (BaseDataSet<TEntity>) set;
        }

        /// <summary>
        ///     Hook to the actual get method of underlying data set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected abstract IBaseDataSet GetDataSet<TEntity>();

        /// <summary>
        ///     Commits the changes of all the stored data sets operations.
        /// </summary>
        public void SaveChanges()
        {
            foreach (var context in _internalContext)
                context.Value.Commit();
        }

        /// <summary>
        ///     Undoes the changes of all the stored data sets operations.
        /// </summary>
        public void Rollback()
        {
            foreach (var context in _internalContext)
                context.Value.Rollback();
        }

        /// <summary>
        ///     Clears the internal context.
        /// </summary>
        public void Clear()
        {
            Dispose();
            _internalContext.Clear();
        }
    }
}