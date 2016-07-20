using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    ///     Represents a data repository of an aggregate type with read operations.
    /// </summary>
    /// <typeparam name="TAggregate">Aggregation type to be read.</typeparam>
    public interface IAggregateReader<out TAggregate>
        where TAggregate : IAggregateRoot
    {
        /// <summary>
        ///     Returns a single instance of the aggregate.
        /// </summary>
        /// <param name="identifiers">List of values used to identify an aggregate.</param>
        /// <returns>Single instance of the aggregate.</returns>
        TAggregate Get(params object[] identifiers);
    }
}