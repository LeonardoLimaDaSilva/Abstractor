using System;
using System.IO;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Minimal representation of a file.
    /// </summary>
    public class GenericFile : IDisposable
    {
        public string FileName { get; }

        public Stream Stream { get; }

        public GenericFile(string fileName, Stream stream)
        {
            Guard.ArgumentIsNotNull(fileName, nameof(fileName));
            Guard.ArgumentIsNotNull(stream, nameof(stream));

            FileName = fileName;
            Stream = stream;
        }

        public virtual void Dispose()
        {
            Stream.Dispose();
        }
    }
}