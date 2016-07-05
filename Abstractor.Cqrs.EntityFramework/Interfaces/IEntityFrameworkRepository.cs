using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.EntityFramework.Interfaces
{
    public interface IEntityFrameworkRepository<TAggregate> : IRepository<TAggregate>
        where TAggregate : AggregateRoot
    {
    }
}