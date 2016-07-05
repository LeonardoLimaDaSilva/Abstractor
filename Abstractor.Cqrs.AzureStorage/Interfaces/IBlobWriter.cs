using Abstractor.Cqrs.AzureStorage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface IBlobWriter<in TEntity> where TEntity : AzureBlob
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);
    }
}