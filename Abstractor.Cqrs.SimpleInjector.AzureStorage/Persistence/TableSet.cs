using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.Persistence;
using Abstractor.Cqrs.SimpleInjector.AzureStorage.Extensions;
using Abstractor.Cqrs.SimpleInjector.AzureStorage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Persistence
{
    public class TableSet<TEntity> : IUnitOfWork
        where TEntity : class, new()
    {
        private readonly List<TableEntityDto> _inserted;
        private readonly List<TableEntityDto> _deleted;
        private readonly List<TableEntityDto> _merged;
        private readonly List<TEntity> _mappedList;
        private readonly CloudTable _table;
        private readonly string _tableName;

        // Quantidade de limite de operações em lote
        private const int MaxBatchOperations = 100;

        public TableSet(string connectionString)
        {
            var tableClient = CloudStorageAccount.Parse(connectionString).CreateCloudTableClient();

            _tableName = typeof(TEntity).GetTableName();
            _table = tableClient.GetTableReference(_tableName);
            _table.CreateIfNotExists();

            _mappedList = new List<TEntity>();
            _inserted = new List<TableEntityDto>();
            _deleted = new List<TableEntityDto>();
            _merged = new List<TableEntityDto>();
        }

        public void Commit()
        {
            ExecuteInserted();
            ExecuteDeleted();
            ExecuteMerged();
        }

        private void ExecuteInserted()
        {
            if (!_inserted.Any()) return;

            var count = 0;

            if (_inserted.Count == 1)
                _table.Execute(TableOperation.Insert(_inserted.First()));
            else
            {
                var batch = new TableBatchOperation();
                foreach (var e in _inserted.Where(i => i.IsDirty))
                    if (count < MaxBatchOperations)
                    {
                        batch.Insert(e);
                        count++;
                    }
                    else
                    {
                        _table.ExecuteBatch(batch);
                        batch = new TableBatchOperation();
                        count = 0;
                    }

                if (batch.Count > 0)
                    _table.ExecuteBatch(batch);
            }
        }

        private void ExecuteDeleted()
        {
            if (!_deleted.Any()) return;

            var count = 0;

            if (_deleted.Count == 1)
                _table.Execute(TableOperation.Delete(_deleted.First()));
            else
            {
                var batch = new TableBatchOperation();
                foreach (var e in _deleted.Where(i => i.IsDirty))
                    if (count < MaxBatchOperations)
                    {
                        batch.Delete(e);
                        count++;
                    }
                    else
                    {
                        _table.ExecuteBatch(batch);
                        batch = new TableBatchOperation();
                        count = 0;
                    }

                if (batch.Count > 0)
                    _table.ExecuteBatch(batch);
            }
        }

        private void ExecuteMerged()
        {
            if (!_merged.Any()) return;

            var count = 0;

            if (_merged.Count == 1)
                _table.Execute(TableOperation.Merge(_merged.First()));
            else
            {
                var batch = new TableBatchOperation();
                foreach (var e in _merged.Where(i => i.IsDirty))
                    if (count < MaxBatchOperations)
                    {
                        batch.Merge(e);
                        count++;
                    }
                    else
                    {
                        _table.ExecuteBatch(batch);
                        batch = new TableBatchOperation();
                        count = 0;
                    }

                if (batch.Count > 0)
                    _table.ExecuteBatch(batch);
            }
        }

        /// <summary>
        /// Obtém uma a entidade que corresponda às chaves informadas.
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey">Se não for informado assume o nome da tabela</param>
        /// <returns></returns>
        public TEntity Get(string partitionKey, string rowKey)
        {
            partitionKey = !string.IsNullOrEmpty(partitionKey)
                ? partitionKey
                : _tableName;

            var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey);
            var filter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, rowKeyFilter);

            var query = new TableQuery().Where(filter);
            var tableEntity = _table.ExecuteQuery(query).SingleOrDefault();
            return tableEntity?.Map<TEntity>();
        }

        /// <summary>
        /// Obtém todos os registros de uma partição utilizando continuation token
        /// </summary>
        /// <param name="partitionKey">Se não for informado assume o nome da tabela</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll(string partitionKey)
        {
            partitionKey = !string.IsNullOrEmpty(partitionKey)
                ? partitionKey
                : _tableName;

            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var query = new TableQuery().Where(filter);

            TableContinuationToken token = null;

            do
            {
                var queryResult = _table.ExecuteQuerySegmented(query, token);
                var result = (queryResult.Results);

                foreach (var item in result)
                    _mappedList.Add(item.Map<TEntity>());

                token = queryResult.ContinuationToken;
            } while (token != null);

            return _mappedList;
        }

        public void Insert(TEntity entity)
        {
            var mapped = CreateDto(entity);

            mapped.IsDirty = true;

            _inserted.Add(mapped);
        }

        public void Delete(TEntity entity)
        {
            var mapped = CreateDto(entity);

            mapped.ETag = "*";
            mapped.IsDirty = true;

            _deleted.Add(mapped);
        }

        public void Merge(TEntity entity)
        {
            var mapped = CreateDto(entity);

            mapped.IsDirty = true;

            _merged.Add(mapped);
        }

        private dynamic CreateDto(object obj)
        {
            var dto = new TableEntityDto();
            string partitionKey = null;
            string rowKey = null;

            var properties = obj
                .GetType()
                .GetProperties()
                .Where(property => property.Name != "PartitionKey" && property.Name != "RowKey");

            foreach (var property in properties)
            {
                if (property.PropertyType.IsCollection())
                    dto.TrySetMember(property.Name, JsonConvert.SerializeObject(property.GetValue(obj, null)));
                else
                {
                    dto.TrySetMember(property.Name, property.GetValue(obj, null) ?? "");

                    var objWithKeys = obj as ITableKeys;

                    if (!string.IsNullOrEmpty(objWithKeys?.PartitionKey))
                        // Se o objeto implementa ITableKeys e possui PartitionKey
                        partitionKey = objWithKeys.PartitionKey;

                    if (!string.IsNullOrEmpty(objWithKeys?.RowKey))
                        // Se o objeto implementa ITableKeys e possui RowKey
                        rowKey = objWithKeys.RowKey;
                    else if (property.Name.ToLower() == "id")
                        // Se o nome da propriedade for Id, assume como RowKey
                        rowKey = property.GetValue(obj, null).ToString();
                }
            }

            // Se nenhuma PartialKey for identificada usa o nome da table
            dto.PartitionKey = partitionKey ?? _tableName;

            // Se nenhuma RowKey for identificada usa um Guid
            dto.RowKey = rowKey ?? Guid.NewGuid().ToString();

            dto.Timestamp = DateTime.Now;

            return dto;
        }
    }
}