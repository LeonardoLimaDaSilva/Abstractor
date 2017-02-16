using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Represents a data repository of an Azure Table TEntity type with write operations.
    /// </summary>
    /// <typeparam name="TEntity">Table entity type to be written.</typeparam>
    public interface ITableWriter<in TEntity>
        where TEntity : ITableEntity
    {
        /// <summary>
        ///     Removes the Azure Table record.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        void Delete(TEntity entity);

        /// <summary>
        ///     Inserts or updates a given Azure Table record.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        void Save(TEntity entity);
    }
}