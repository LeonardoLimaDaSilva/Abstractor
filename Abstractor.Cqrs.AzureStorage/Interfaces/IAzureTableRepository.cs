using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Represents a repository of an Azure Table.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IAzureTableRepository<TEntity> : ITableWriter<TEntity>, ITableReader<TEntity>
        where TEntity : ITableEntity
    {
    }
}