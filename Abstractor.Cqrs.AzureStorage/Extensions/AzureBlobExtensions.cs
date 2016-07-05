using Abstractor.Cqrs.AzureStorage.Blob;
using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.AzureStorage.Extensions
{
    /// <summary>
    /// Extensões de <see cref="AzureBlob"/>.
    /// </summary>
    public static class AzureBlobExtensions
    {
        /// <summary>
        /// Converte um <see cref="AzureBlob"/> em <see cref="GenericFile"/>.
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public static GenericFile ToGenericFile(this AzureBlob blob)
        {
            return new GenericFile(blob.FileName, blob.Stream);
        }
    }
}