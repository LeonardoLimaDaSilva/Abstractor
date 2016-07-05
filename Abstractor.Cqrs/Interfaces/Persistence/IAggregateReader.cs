using System.Linq;
using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    ///     Especifica um repositório de dados a retornar um conjunto de instâncias de agregação.
    /// </summary>
    /// <typeparam name="TAggregate">O tipo da agregação que será retornada.</typeparam>
    public interface IAggregateReader<out TAggregate>
        where TAggregate : IAggregateRoot
    {
        /// <summary>
        ///     Retorna um conjunto de instâncias de agregação.
        /// </summary>
        /// <returns>IQueryable do conjunto de instâncias de agregação.</returns>
        IQueryable<TAggregate> Query();

        /// <summary>
        ///     Retorna uma instância única da agregação.
        /// </summary>
        /// <param name="primaryKey">Valores de chave primária da instância da agregação.</param>
        /// <returns>Instância única da agregação.</returns>
        TAggregate Get(params object[] primaryKey);
    }
}