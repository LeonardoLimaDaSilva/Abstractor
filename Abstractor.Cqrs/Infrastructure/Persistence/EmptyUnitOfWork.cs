using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     Unidade de trabalho vazia utilizada quando nenhuma outra implementação for explicitamente definida.
    /// </summary>
    public class EmptyUnitOfWork : IUnitOfWork
    {
        public void Commit()
        {
        }
    }
}