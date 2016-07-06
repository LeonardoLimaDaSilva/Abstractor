using System;
using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    public abstract class BaseBlobRepository<TEntity> : IFileRepository where TEntity : AzureBlob, new()
    {
        private readonly IAzureBlobRepository<TEntity> _repository;

        protected BaseBlobRepository(IAzureBlobRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public void Save(GenericFile file)
        {
            _repository.Save(CreateInstance(file));
        }

        public void Delete(GenericFile file)
        {
            _repository.Delete(CreateInstance(file));
        }

        public GenericFile Get(string fileName)
        {
            return _repository.Get(fileName).ToGenericFile();
        }

        private static TEntity CreateInstance(GenericFile file)
        {
            return (TEntity) Activator.CreateInstance(typeof (TEntity), file);
        }
    }
}