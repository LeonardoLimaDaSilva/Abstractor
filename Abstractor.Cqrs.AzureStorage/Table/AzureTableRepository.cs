using System;
using System.Collections.Generic;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Table
{
    /// <summary>
    ///     Repository implementation specific for Azure Table.
    /// </summary>
    /// <typeparam name="TEntity">Table entity type.</typeparam>
    internal sealed class AzureTableRepository<TEntity> : IAzureTableRepository<TEntity>
        where TEntity : ITableEntity, new()
    {
        private readonly Func<AzureTableContext> _contextProvider;

        public AzureTableRepository(Func<AzureTableContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        /// <summary>
        ///     Verifies whether an Azure Table exists with the specified row and partition keys.
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public bool Exists(string rowKey, string partitionKey = null)
        {
            var tbset = (AzureTableSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.Get(rowKey, partitionKey) != null;
        }

        /// <summary>
        ///     Inserts or updates a given Azure Table record.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        public void Save(TEntity entity)
        {
            var tbset = _contextProvider().Set<TEntity>();

            if (Exists(entity.RowKey, entity.PartitionKey))
                tbset.Update(entity);
            else
                tbset.Insert(entity);
        }

        /// <summary>
        ///     Removes the Azure Table record.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        public void Delete(TEntity entity)
        {
            var tbset = _contextProvider().Set<TEntity>();
            tbset.Delete(entity);
        }

        /// <summary>
        ///     Gets all records from the table.
        /// </summary>
        /// <param name="partitionKey">Optional partition key.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll(string partitionKey = null)
        {
            var tbset = (AzureTableSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.GetAll(partitionKey);
        }

        /// <summary>
        ///     Gets a single record from the table, matching the given keys.
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public TEntity Get(string rowKey, string partitionKey = null)
        {
            var tbset = (AzureTableSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.Get(rowKey, partitionKey);
        }
    }
}