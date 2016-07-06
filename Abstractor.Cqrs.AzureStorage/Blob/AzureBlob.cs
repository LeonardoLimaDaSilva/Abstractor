using System;
using System.IO;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    /// <summary>
    ///     Representação simplificada de um blob.
    /// </summary>
    public class AzureBlob : IDisposable
    {
        public string FileName { get; set; }

        public Stream Stream { get; set; }

        public void Dispose()
        {
            Stream.Dispose();
        }

        public AzureBlob()
        {
        }

        public AzureBlob(GenericFile file)
        {
            Guard.ArgumentIsNotNull(file, nameof(file));

            FileName = file.FileName;
            Stream = file.Stream;
        }

        public GenericFile ToGeneric()
        {
            return new GenericFile(FileName, Stream);
        }
    }
}