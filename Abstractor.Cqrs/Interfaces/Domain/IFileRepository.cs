using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.Interfaces.Domain
{
    /// <summary>
    ///     Defines a generic file repository.
    /// </summary>
    public interface IFileRepository
    {
        void Save(GenericFile file);
        void Delete(GenericFile file);
        GenericFile Get(string fileName);
    }
}