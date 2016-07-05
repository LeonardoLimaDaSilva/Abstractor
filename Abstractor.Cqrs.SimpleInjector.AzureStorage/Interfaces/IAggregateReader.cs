using System.Collections.Generic;
using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Interfaces
{
    /// <summary>
    /// Especifica um repositório de dados a retornar um conjunto de instâncias de agregação com permissão de leitura apenas.
    /// </summary>
    /// <typeparam name="TAggregate">O tipo da agregação que retornará instâncias com permissão de leitura apenas.</typeparam>
    public interface IAggregateReader<out TAggregate>
        where TAggregate : IAggregateRoot
    {
        /// <summary>
        /// Retorna um todas as instâncias da agregação com permissão de leitura apenas.
        /// </summary>
        /// <param name="partitionKey">Se não for informado assume o nome da tabela.</param>
        /// <returns>IEnumerable do conjunto de instâncias de agregação com permissão de leitura apenas.</returns>
        IEnumerable<TAggregate> GetAll(object partitionKey = null);

        /// <summary>
        /// Retorna uma instância única da agregação com permissão de escrita.
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey">Se não for informado assume o nome da tabela.</param>
        /// <returns>
        /// Uma instância única da agregação com permissão de escrita que corresponda aos 
        /// parâmetros <paramref name="partitionKey"/> e <paramref name="rowKey"/>, caso contrário retorna nulo.
        /// </returns>
        TAggregate Get(object rowKey, object partitionKey = null);
    }
}
