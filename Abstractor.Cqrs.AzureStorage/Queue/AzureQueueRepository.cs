using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    internal sealed class AzureQueueRepository<TEntity> : IAzureQueueRepository<TEntity>
        where TEntity : AzureQueueMessage, new()
    {
        private readonly Func<AzureQueueContext> _contextProvider;

        public AzureQueueRepository(Func<AzureQueueContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public void Add(TEntity entity)
        {
            var tbset = _contextProvider().Set<TEntity>();
            tbset.Insert(entity);
        }

        public void Delete(TEntity entity)
        {
            var tbset = _contextProvider().Set<TEntity>();
            tbset.Delete(entity);
        }

        public TEntity GetNext(TimeSpan? visibilityTimeout = null)
        {
            var tbset = (AzureQueueSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.GetNext(visibilityTimeout);
        }

        public int Count()
        {
            var tbset = (AzureQueueSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.Count();
        }
    }
}