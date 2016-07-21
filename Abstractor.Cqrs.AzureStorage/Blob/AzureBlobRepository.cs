using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    /// <summary>
    ///     Repository implementation specific for Azure Blob.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal sealed class AzureBlobRepository<TEntity> : IAzureBlobRepository<TEntity>
        where TEntity : AzureBlob, new()
    {
        private readonly AzureBlobContext _context;

        public AzureBlobRepository(IAzureBlobContext context)
        {
            _context = (AzureBlobContext)context;
        }

        /// <summary>
        ///     Verifies whether an Azure Blob exists with the specified filename in the container.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns></returns>
        public bool Exists(string fileName)
        {
            var set = (AzureBlobSet<TEntity>) _context.Set<TEntity>();
            return set.Exists(fileName);
        }

        /// <summary>
        ///     Inserts or updates a given Azure Blob into the container.
        /// </summary>
        /// <param name="entity">Entity to be saved.</param>
        public void Save(TEntity entity)
        {
            var set = _context.Set<TEntity>();

            if (!Exists(entity.FileName))
                set.Insert(entity);
            else
                set.Update(entity);
        }

        /// <summary>
        ///     Removes the Azure Blob from it's container.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        public void Delete(TEntity entity)
        {
            var set = _context.Set<TEntity>();
            set.Delete(entity);
        }

        /// <summary>
        ///     Gets the Azure Blob with the specified filename from the container.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns></returns>
        public TEntity Get(string fileName)
        {
            var set = (AzureBlobSet<TEntity>) _context.Set<TEntity>();
            return set.Get(fileName);
        }

        /// <summary>
        ///     Gets the virtual path for the current filename.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns>Uri virtual path.</returns>
        public Uri GetVirtualPath(string fileName)
        {
            var set = (AzureBlobSet<TEntity>) _context.Set<TEntity>();
            return set.GetVirtualPath(fileName);
        }
    }
}