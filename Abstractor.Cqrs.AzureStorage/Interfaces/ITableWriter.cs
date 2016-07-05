using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface ITableWriter<in TEntity> where TEntity : ITableEntity
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);
    }
}