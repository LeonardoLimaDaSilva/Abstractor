using System;
using System.IO;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    /// <summary>
    ///     Minimal representation of an Azure Blob.
    /// </summary>
    public class AzureBlob : IDisposable
    {
        /// <summary>
        ///     The name of blob.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Stream representation of the blob.
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public AzureBlob()
        {
        }

        /// <summary>
        ///     Constructs based on a generic file.
        /// </summary>
        /// <param name="file">Representation of a file.</param>
        public AzureBlob(GenericFile file)
        {
            Guard.ArgumentIsNotNull(file, nameof(file));

            FileName = file.FileName;
            Stream = file.Stream;
        }

        /// <summary>
        ///     Disposes the stream.
        /// </summary>
        public void Dispose()
        {
            Stream.Dispose();
        }

        /// <summary>
        ///     Converts to a generic file.
        /// </summary>
        /// <returns></returns>
        public GenericFile ToGeneric()
        {
            return new GenericFile(FileName, Stream);
        }
    }
}