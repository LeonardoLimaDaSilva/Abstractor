using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.Interfaces.Domain
{
    /// <summary>
    ///     Define um reposit�rio gen�rico de arquivos.
    /// </summary>
    public interface IFileRepository
    {
        void Save(GenericFile file);
        void Delete(GenericFile file);
        GenericFile Get(string fileName);
    }
}