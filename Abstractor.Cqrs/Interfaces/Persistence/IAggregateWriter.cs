using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    ///     Represents a data repository of an aggregate type with write operations.
    /// </summary>
    /// <typeparam name="TAggregate">Aggregation type to be written.</typeparam>
    public interface IAggregateWriter<in TAggregate>
        where TAggregate : IAggregateRoot
    {
        /// <summary>
        ///     Adds a new instance to the set.
        /// </summary>
        /// <param name="aggregate">Aggregate to be created.</param>
        void Create(TAggregate aggregate);

        /// <summary>
        ///     Removes an existing aggregation from the set.
        /// </summary>
        /// <param name="aggregate">Aggregate to be removed.</param>
        void Delete(TAggregate aggregate);

        /// <summary>
        ///     Modifies the state of an existing aggregate.
        /// </summary>
        /// <param name="aggregate">Aggregate to be modified.</param>
        void Update(TAggregate aggregate);
    }
}