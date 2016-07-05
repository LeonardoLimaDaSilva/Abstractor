using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    public abstract class BaseBlobRepository<TEntity> : IFileRepository where TEntity : AzureBlob
    {
        private readonly IAzureBlobRepository<TEntity> _repository;

        protected BaseBlobRepository(IAzureBlobRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public void Save(GenericFile file)
        {
            _repository.Save(ToEntity(file));
        }

        public void Delete(GenericFile file)
        {
            _repository.Delete(ToEntity(file));
        }

        public GenericFile Get(string fileName)
        {
            return _repository.Get(fileName).ToGenericFile();
        }

        protected abstract TEntity ToEntity(GenericFile file);
    }
}