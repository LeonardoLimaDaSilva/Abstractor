using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface ITableWriter<in TEntity> 
        where TEntity : ITableEntity
    {
        /// <summary>
        ///     Inserts or updates a given Azure Table record.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        void Save(TEntity entity);

        /// <summary>
        ///     Removes the Azure Table record.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        void Delete(TEntity entity);
    }
}