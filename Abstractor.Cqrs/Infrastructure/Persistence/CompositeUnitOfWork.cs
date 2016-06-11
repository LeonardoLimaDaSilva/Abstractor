using System.Collections.Generic;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    /// Unidade de trabalho composta utilizada quando existe a necessidade de usar mais de uma implementação de <see cref="IUnitOfWork"/>.
    /// </summary>
    public class CompositeUnitOfWork : IUnitOfWork
    {
        private readonly IEnumerable<IUnitOfWork> _unitsOfWork;

        public CompositeUnitOfWork(IEnumerable<IUnitOfWork> unitsOfWork)
        {
            _unitsOfWork = unitsOfWork;
        }

        public void Commit()
        {
            foreach (var uow in _unitsOfWork)
            {
                uow.Commit();
            }
        }
    }
}
