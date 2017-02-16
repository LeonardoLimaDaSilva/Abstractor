using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.Interfaces.Domain
{
    /// <summary>
    ///     Defines a generic file repository.
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        ///     Deletes the file from the underlying repository.
        /// </summary>
        /// <param name="file">Generic file.</param>
        void Delete(GenericFile file);

        /// <summary>
        ///     Retrieves the generic file representation.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns></returns>
        GenericFile Get(string fileName);

        /// <summary>
        ///     Saves the file in the underlying repository.
        /// </summary>
        /// <param name="file">Generic file.</param>
        void Save(GenericFile file);
    }
}