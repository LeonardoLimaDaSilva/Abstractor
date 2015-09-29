using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    /// Especifica um repositório de dados a ser retornado para o tipo de agregação informada.
    /// </summary>
    /// <typeparam name="TAggregate">A agregação que define o tipo do repositório.</typeparam>
    public interface IRepository<TAggregate> : IAggregateWriter<TAggregate>, IAggregateReader<TAggregate> where TAggregate : IAggregateRoot
    {
        /// <summary>
        /// Retorna uma instância única da agregação com permissão de escrita.
        /// </summary>
        /// <param name="primaryKey">Identificador único da instância da agregação.</param>
        /// <returns>Uma instância única da agregação com permissão de escrita que corresponda ao <paramref name="primaryKey"/>, caso contrário retorna nulo.</returns>
        TAggregate Get(object primaryKey);
    }
}
