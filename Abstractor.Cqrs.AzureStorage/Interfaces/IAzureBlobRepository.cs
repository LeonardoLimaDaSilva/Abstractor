using Abstractor.Cqrs.AzureStorage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Represents a repository of an Azure Blob.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IAzureBlobRepository<TEntity> : IBlobWriter<TEntity>, IBlobReader<TEntity>
        where TEntity : AzureBlob
    {
    }
}