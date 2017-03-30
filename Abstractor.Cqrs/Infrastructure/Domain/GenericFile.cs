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
        /// <summary>
        ///     Name of the file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        ///     Stream representation of the file.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        ///     Default empty constructor.
        /// </summary>
        public GenericFile()
        {
            Stream = Stream.Null;
        }

        /// <summary>
        ///     Constructs a generic file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">Stream representation of the file.</param>
        public GenericFile(string fileName, Stream stream)
        {
            Guard.ArgumentIsNotNull(fileName, nameof(fileName));
            Guard.ArgumentIsNotNull(stream, nameof(stream));

            FileName = fileName;
            Stream = stream;
        }

        /// <summary>
        ///     Releases the generic file stream resources.
        /// </summary>
        public virtual void Dispose()
        {
            Stream.Dispose();
        }
    }
}