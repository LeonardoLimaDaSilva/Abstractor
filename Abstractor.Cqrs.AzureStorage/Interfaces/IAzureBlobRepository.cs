using Abstractor.Cqrs.AzureStorage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface IAzureBlobRepository<TEntity> : IBlobWriter<TEntity>, IBlobReader<TEntity>
        where TEntity : AzureBlob
    {
    }
}