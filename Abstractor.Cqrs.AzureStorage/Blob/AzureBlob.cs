using System;
using System.IO;

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
    }
}