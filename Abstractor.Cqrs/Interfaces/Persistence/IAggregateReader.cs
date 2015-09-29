using System.Linq;
using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    /// Especifica um repositório de dados a retornar um conjunto de instâncias de agregação com permissão de leitura apenas.
    /// </summary>
    /// <typeparam name="TAggregate">O tipo da agregação que retornará instâncias com permissão de leitura apenas.</typeparam>
    public interface IAggregateReader<out TAggregate> where TAggregate : IAggregateRoot
    {
        /// <summary>
        /// Retorna um conjunto de instâncias de agregação com permissão de leitura apenas.
        /// </summary>
        /// <returns>IQueryable do conjunto de instâncias de agregação com permissão de leitura apenas.</returns>
        IQueryable<TAggregate> Query();
    }
}
