using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface ITableReader<out TEntity> where TEntity : ITableEntity
    {
        /// <summary>
        ///     Verifies whether an Azure Table exists with the specified row and partition keys.
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        bool Exists(string rowKey, string partitionKey = null);

        /// <summary>
        ///     Gets all records from the table.
        /// </summary>
        /// <param name="partitionKey">Optional partition key.</param>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll(string partitionKey = null);

        /// <summary>
        ///     Gets a single record from the table, matching the given keys.
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        TEntity Get(string rowKey, string partitionKey = null);
    }
}