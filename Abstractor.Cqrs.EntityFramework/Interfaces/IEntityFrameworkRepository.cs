using System.Linq;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.EntityFramework.Interfaces
{
    /// <summary>
    ///     Represents a data repository of an aggregate type specific for Entity Framework.
    /// </summary>
    /// <typeparam name="TAggregate">Aggregate type.</typeparam>
    public interface IEntityFrameworkRepository<TAggregate> : IRepository<TAggregate>
        where TAggregate : AggregateRoot
    {
        /// <summary>
        ///     Returns a queryable list of instances of aggregate.
        /// </summary>
        /// <returns>Queryable list of aggregates.</returns>
        IQueryable<TAggregate> Query();
    }
}