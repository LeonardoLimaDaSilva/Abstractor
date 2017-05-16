using System;
using System.IO;
using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    /// <summary>
    ///     Azure Blob specific implementation of a data set.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public sealed class AzureBlobSet<TEntity> : BaseDataSet<TEntity>
        where TEntity : AzureBlob, new()
    {
        private readonly CloudBlobContainer _container;

        /// <summary>
        ///     AzureBlobSet constructor.
        /// </summary>
        /// <param name="connectionString">Azure connection string.</param>
        public AzureBlobSet(string connectionString)
        {
            var blobClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();

            _container = blobClient.GetContainerReference(typeof(TEntity).GetContainerName());
            _container.CreateIfNotExists(typeof(TEntity).GetPublicAccessType());
        }

        /// <summary>
        ///     Verifies whether an Azure Blob exists with the specified filename in the container.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns></returns>
        public bool Exists(string fileName)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);
            return blockBlob.Exists();
        }

        /// <summary>
        ///     Downloads the Azure Blob from the container.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns>Azure Blob.</returns>
        public TEntity Get(string fileName)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);
            if (!blockBlob.Exists()) return null;

            var memoryStream = new MemoryStream();
            blockBlob.DownloadToStream(memoryStream);

            return new TEntity
            {
                Stream = memoryStream,
                FileName = fileName
            };
        }

        /// <summary>
        ///     Gets the virtual path for the current filename.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns>Uri virtual path.</returns>
        public Uri GetVirtualPath(string fileName)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);
            return blockBlob.Uri;
        }

        /// <summary>
        ///     Removes the Azure Blob from the container.
        /// </summary>
        /// <param name="entity">Entity to be deleted.</param>
        protected override void DeleteEntity(TEntity entity)
        {
            var blobToDelete = _container.GetBlockBlobReference(entity.FileName);
            blobToDelete.Delete();
        }

        /// <summary>
        ///     Downloads the Azure Blob from the container.
        /// </summary>
        /// <param name="entity">Entity definition.</param>
        /// <returns>Azure Blob.</returns>
        protected override TEntity Get(TEntity entity)
        {
            return Get(entity.FileName);
        }

        /// <summary>
        ///     Uploads the Azure Blob stream to the container.
        /// </summary>
        /// <param name="entity">Entity to be inserted.</param>
        protected override void InsertEntity(TEntity entity)
        {
            var blobToUpload = _container.GetBlockBlobReference(entity.FileName);

            var cacheControl = typeof(TEntity).GetBlobCacheControl();
            var extension = Path.GetExtension(entity.FileName) ?? string.Empty;

            blobToUpload.Properties.ContentType = MimeTypeMapper.GetFromExtension(extension.ToLower());
            blobToUpload.Properties.CacheControl = cacheControl;

            blobToUpload.SetProperties();

            blobToUpload.UploadFromStream(entity.Stream);
        }

        /// <summary>
        ///     Overwrites the Azure Blob into the container.
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        protected override void UpdateEntity(TEntity entity)
        {
            InsertEntity(entity);
        }
    }
}