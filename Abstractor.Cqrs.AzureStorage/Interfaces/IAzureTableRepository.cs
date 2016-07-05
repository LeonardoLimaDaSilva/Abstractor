using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface IAzureTableRepository<TEntity> : ITableWriter<TEntity>, ITableReader<TEntity>
        where TEntity : ITableEntity
    {
    }
}