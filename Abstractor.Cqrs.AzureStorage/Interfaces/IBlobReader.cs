using System;
using Abstractor.Cqrs.AzureStorage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Defines the read operations of an Azure Blob repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IBlobReader<out TEntity> 
        where TEntity : AzureBlob
    {
        /// <summary>
        ///     Verifies whether an Azure Blob exists with the specified filename in the container.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns></returns>
        bool Exists(string fileName);

        /// <summary>
        ///     Gets the Azure Blob with the specified filename from the container.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns></returns>
        TEntity Get(string fileName);

        /// <summary>
        ///     Gets the virtual path for the current filename.
        /// </summary>
        /// <param name="fileName">Azure Blob filename.</param>
        /// <returns>Uri virtual path.</returns>
        Uri GetVirtualPath(string fileName);
    }
}