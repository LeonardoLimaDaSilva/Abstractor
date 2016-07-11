using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     Default unit of work, used when there is no explicit implementation defined.
    /// </summary>
    public class EmptyUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Commit()
        {
        }
    }
}