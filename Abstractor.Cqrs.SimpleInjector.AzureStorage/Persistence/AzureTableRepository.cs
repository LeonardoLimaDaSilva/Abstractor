using System;
using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.SimpleInjector.AzureStorage.Interfaces;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Persistence
{
    internal sealed class AzureTableRepository<TAggregate> : IAzureTableRepository<TAggregate>
        where TAggregate : AggregateRoot, new()
    {
        private readonly Func<TsContext> _contextProvider;

        public AzureTableRepository(Func<TsContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public void Create(TAggregate entity)
        {
            var dbset = _contextProvider().Set<TAggregate>();
            dbset.Insert(entity);
        }

        public void Delete(TAggregate entity)
        {
            var dbset = _contextProvider().Set<TAggregate>();
            dbset.Delete(entity);
        }

        public void Update(TAggregate entity)
        {
            var dbset = _contextProvider().Set<TAggregate>();
            dbset.Merge(entity);
        }

        public IEnumerable<TAggregate> GetAll(object partitionKey = null)
        {
            var dbset = _contextProvider().Set<TAggregate>();
            return dbset.GetAll(partitionKey as string);
        }

        public TAggregate Get(object rowKey, object partitionKey = null)
        {
            var dbset = _contextProvider().Set<TAggregate>();
            return dbset.Get(partitionKey?.ToString(), rowKey?.ToString());
        }
    }
}
