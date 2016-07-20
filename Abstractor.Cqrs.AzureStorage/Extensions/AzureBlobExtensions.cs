using Abstractor.Cqrs.AzureStorage.Blob;
using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.AzureStorage.Extensions
{
    /// <summary>
    ///     Extensions for <see cref="AzureBlob" />.
    /// </summary>
    public static class AzureBlobExtensions
    {
        /// <summary>
        ///     Converts an <see cref="AzureBlob" /> to a <see cref="GenericFile" />.
        /// </summary>
        /// <param name="blob">Azure Blob.</param>
        /// <returns>Generic file.</returns>
        public static GenericFile ToGenericFile(this AzureBlob blob)
        {
            return new GenericFile(blob.FileName, blob.Stream);
        }
    }
}