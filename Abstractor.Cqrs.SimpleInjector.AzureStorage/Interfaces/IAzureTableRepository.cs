using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Interfaces
{
    /// <summary>
    /// Especifica um repositório de dados a ser retornado para o tipo de agregação informada.
    /// </summary>
    /// <typeparam name="TAggregate">A agregação que define o tipo do repositório.</typeparam>
    public interface IAzureTableRepository<TAggregate> : IAggregateWriter<TAggregate>, IAggregateReader<TAggregate>
        where TAggregate : IAggregateRoot
    {
    }
}