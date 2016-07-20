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
        public string FileName { get; set; }

        public Stream Stream { get; set; }

        public AzureBlob()
        {
        }

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