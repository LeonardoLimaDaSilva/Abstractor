using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    internal sealed class AzureBlobRepository<TEntity> : IAzureBlobRepository<TEntity> where TEntity : AzureBlob, new()
    {
        private readonly Func<AzureBlobContext> _contextProvider;

        public AzureBlobRepository(Func<AzureBlobContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public bool Exists(string fileName)
        {
            var dataSet = (AzureBlobSet<TEntity>) _contextProvider().Set<TEntity>();
            return dataSet.Exists(fileName);
        }

        public void Save(TEntity entity)
        {
            var dataSet = _contextProvider().Set<TEntity>();

            if (!Exists(entity.FileName))
                dataSet.Insert(entity);
            else
                dataSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            var dataSet = _contextProvider().Set<TEntity>();
            dataSet.Delete(entity);
        }

        public TEntity Get(string fileName)
        {
            var dataSet = (AzureBlobSet<TEntity>) _contextProvider().Set<TEntity>();
            return dataSet.Get(fileName);
        }

        public Uri GetVirtualPath(string fileName)
        {
            var dataSet = (AzureBlobSet<TEntity>) _contextProvider().Set<TEntity>();
            return dataSet.GetVirtualPath(fileName);
        }
    }
}