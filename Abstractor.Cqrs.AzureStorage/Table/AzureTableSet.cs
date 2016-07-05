using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Table
{
    public sealed class AzureTableSet<TEntity> : BaseDataSet<TEntity> where TEntity : ITableEntity, new()
    {
        private readonly CloudTable _table;
        private readonly string _tableName;

        public AzureTableSet(string connectionString)
        {
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));

            var tableClient = CloudStorageAccount.Parse(connectionString).CreateCloudTableClient();
            _tableName = typeof (TEntity).GetTableName();
            _table = tableClient.GetTableReference(_tableName);
            _table.CreateIfNotExists();
        }

        protected override void InsertEntity(TEntity entity)
        {
            Guard.ArgumentIsNotNull(entity, nameof(entity));

            entity.PartitionKey = entity.PartitionKey ?? _tableName;
            entity.ETag = null;
            _table.Execute(TableOperation.Insert(entity));
        }

        protected override void DeleteEntity(TEntity entity)
        {
            Guard.ArgumentIsNotNull(entity, nameof(entity));

            entity.PartitionKey = entity.PartitionKey ?? _tableName;
            entity.ETag = "*";
            _table.Execute(TableOperation.Delete(entity));
        }

        protected override void UpdateEntity(TEntity entity)
        {
            Guard.ArgumentIsNotNull(entity, nameof(entity));

            entity.PartitionKey = entity.PartitionKey ?? _tableName;
            entity.ETag = "*";
            _table.Execute(TableOperation.Merge(entity));
        }

        protected override TEntity Get(TEntity entity)
        {
            Guard.ArgumentIsNotNull(entity, nameof(entity));

            entity.PartitionKey = entity.PartitionKey ?? _tableName;
            return Get(entity.RowKey, entity.PartitionKey);
        }

        public TEntity Get(string rowKey, string partitionKey)
        {
            Guard.ArgumentIsNotNull(rowKey, nameof(rowKey));

            partitionKey = !string.IsNullOrEmpty(partitionKey)
                ? partitionKey
                : _tableName;

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.Equal,
                partitionKey);
            var rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey);
            var filter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, rowKeyFilter);

            var query = new TableQuery<TEntity>().Where(filter);
            return _table.ExecuteQuery(query).SingleOrDefault();
        }

        public IEnumerable<TEntity> GetAll(string partitionKey)
        {
            partitionKey = !string.IsNullOrEmpty(partitionKey)
                ? partitionKey
                : _tableName;

            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var query = new TableQuery<TEntity>().Where(filter);

            TableContinuationToken token = null;

            var list = new List<TEntity>();

            do
            {
                var queryResult = _table.ExecuteQuerySegmented(query, token);
                var results = (queryResult.Results);

                list.AddRange(results);

                token = queryResult.ContinuationToken;
            } while (token != null);

            return list;
        }
    }
}