using System;
using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    /// <summary>
    ///     Base repository implementation specific for Azure Blob.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public abstract class BaseBlobRepository<TEntity> : IFileRepository
        where TEntity : AzureBlob, new()
    {
        private readonly IAzureBlobRepository<TEntity> _repository;

        /// <summary>
        ///     BaseBlobRepository constructor.
        /// </summary>
        /// <param name="repository">Repository of an Azure Blob.</param>
        protected BaseBlobRepository(IAzureBlobRepository<TEntity> repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Converts a generic file into an Azure Blob and removes from the container.
        /// </summary>
        /// <param name="file">File to be deleted.</param>
        public void Delete(GenericFile file)
        {
            _repository.Delete(CreateInstance(file));
        }

        /// <summary>
        ///     Gets an Azure Blob with the specified file name from the container and converts to a generic file.
        /// </summary>
        /// <param name="fileName">Name of file to be returned.</param>
        /// <returns></returns>
        public GenericFile Get(string fileName)
        {
            return _repository.Get(fileName).ToGenericFile();
        }

        /// <summary>
        ///     Converts a generic file into an Azure Blob and saves into the container.
        /// </summary>
        /// <param name="file">File to be saved.</param>
        public void Save(GenericFile file)
        {
            _repository.Save(CreateInstance(file));
        }

        /// <summary>
        ///     Gets an Uri that contains the virtual path of the specified file.
        /// </summary>
        /// <param name="fileName">Name that identifies the file.</param>
        /// <returns></returns>
        public Uri GetVirtualPath(string fileName)
        {
            return _repository.GetVirtualPath(fileName);
        }

        /// <summary>
        ///     Creates a new instance of TEntity passing the generic file as parameter.
        /// </summary>
        /// <param name="file">Generic file.</param>
        /// <returns></returns>
        private static TEntity CreateInstance(GenericFile file)
        {
            return (TEntity) Activator.CreateInstance(typeof(TEntity), file);
        }
    }
}