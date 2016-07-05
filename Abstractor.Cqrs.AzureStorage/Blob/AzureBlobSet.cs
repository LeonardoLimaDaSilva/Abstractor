using System;
using System.IO;
using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    public sealed class AzureBlobSet<TEntity> : BaseDataSet<TEntity> where TEntity : AzureBlob, new()
    {
        private readonly CloudBlobContainer _container;

        public AzureBlobSet(string connectionString)
        {
            var blobClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(typeof (TEntity).GetContainerName());
            _container.CreateIfNotExists();
        }

        protected override void InsertEntity(TEntity entity)
        {
            var blobToUpload = _container.GetBlockBlobReference(entity.FileName);
            blobToUpload.UploadFromStream(entity.Stream);
        }

        protected override void DeleteEntity(TEntity entity)
        {
            var blobToDelete = _container.GetBlockBlobReference(entity.FileName);
            blobToDelete.Delete();
        }

        protected override void UpdateEntity(TEntity entity)
        {
            InsertEntity(entity);
        }

        protected override TEntity Get(TEntity entity)
        {
            return Get(entity.FileName);
        }

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

        public Uri GetVirtualPath(string fileName)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);
            return blockBlob.Uri;
        }

        public bool Exists(string fileName)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);
            return blockBlob.Exists();
        }
    }
}