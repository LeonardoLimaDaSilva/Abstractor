using Abstractor.Cqrs.AzureStorage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Defines the write operations of an Azure Blob repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IBlobWriter<in TEntity>
        where TEntity : AzureBlob
    {
        /// <summary>
        ///     Removes the Azure Blob from it's container.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        void Delete(TEntity entity);

        /// <summary>
        ///     Inserts or updates a given Azure Blob into the container.
        /// </summary>
        /// <param name="entity">Entity to be saved.</param>
        void Save(TEntity entity);
    }
}