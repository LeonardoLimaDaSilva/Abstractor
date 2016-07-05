using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface ITableReader<out TEntity> where TEntity : ITableEntity
    {
        bool Exists(string rowKey, string partitionKey = null);
        IEnumerable<TEntity> GetAll(string partitionKey = null);
        TEntity Get(string rowKey, string partitionKey = null);
    }
}