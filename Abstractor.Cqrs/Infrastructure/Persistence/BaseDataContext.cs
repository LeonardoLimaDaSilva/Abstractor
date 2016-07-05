using System;
using System.Collections.Generic;
using System.Linq;

namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    public abstract class BaseDataContext : IDisposable
    {
        private readonly IDictionary<string, IBaseDataSet> _internalContext;

        protected BaseDataContext()
        {
            _internalContext = new Dictionary<string, IBaseDataSet>();
        }

        public void Dispose()
        {
            foreach (var d in _internalContext.Values.Select(dataSet => dataSet as IDisposable))
                d?.Dispose();
        }

        public BaseDataSet<TEntity> Set<TEntity>()
        {
            if (_internalContext.ContainsKey(typeof (TEntity).Name))
                return (BaseDataSet<TEntity>) _internalContext[typeof (TEntity).Name];

            var set = GetDataSet<TEntity>();

            _internalContext.Add(typeof (TEntity).Name, set);

            return (BaseDataSet<TEntity>) set;
        }

        protected abstract IBaseDataSet GetDataSet<TEntity>();

        public void SaveChanges()
        {
            foreach (var context in _internalContext)
                context.Value.Commit();
        }

        public void Rollback()
        {
            foreach (var context in _internalContext)
                context.Value.Rollback();
        }
    }
}