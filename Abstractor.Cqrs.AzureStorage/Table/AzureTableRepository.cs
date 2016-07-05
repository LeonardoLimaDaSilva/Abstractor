using System;
using System.Collections.Generic;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Table
{
    internal sealed class AzureTableRepository<TEntity> : IAzureTableRepository<TEntity>
        where TEntity : ITableEntity, new()
    {
        private readonly Func<AzureTableContext> _contextProvider;

        public AzureTableRepository(Func<AzureTableContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public bool Exists(string rowKey, string partitionKey = null)
        {
            var tbset = (AzureTableSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.Get(rowKey, partitionKey) != null;
        }

        public void Save(TEntity entity)
        {
            var tbset = _contextProvider().Set<TEntity>();

            if (Exists(entity.RowKey, entity.PartitionKey))
                tbset.Update(entity);
            else
                tbset.Insert(entity);
        }

        public void Delete(TEntity entity)
        {
            var tbset = _contextProvider().Set<TEntity>();
            tbset.Delete(entity);
        }

        public IEnumerable<TEntity> GetAll(string partitionKey = null)
        {
            var tbset = (AzureTableSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.GetAll(partitionKey);
        }

        public TEntity Get(string rowKey, string partitionKey = null)
        {
            var tbset = (AzureTableSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.Get(rowKey, partitionKey);
        }
    }
}