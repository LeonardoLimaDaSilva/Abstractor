using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    ///     Represents a data repository of an aggregate type.
    /// </summary>
    /// <typeparam name="TAggregate">Aggregate type.</typeparam>
    public interface IRepository<TAggregate> : IAggregateWriter<TAggregate>, IAggregateReader<TAggregate>
        where TAggregate : IAggregateRoot
    {
    }
}