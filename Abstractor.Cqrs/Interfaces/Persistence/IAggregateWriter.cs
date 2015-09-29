using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    /// Especifica um repositório de dados a aceitar um conjunto de instâncias de agregação com permissão de escrita.
    /// </summary>
    /// <typeparam name="TAggregate">A agregação</typeparam>
    public interface IAggregateWriter<in TAggregate> where TAggregate : IAggregateRoot
    {
        /// <summary>
        /// Adiciona uma nova instância de agregação no conjunto de instâncias.
        /// </summary>
        /// <param name="aggregate">Agregação que será adicionada.</param>
        void Create(TAggregate aggregate);

        /// <summary>
        /// Remove permanentemente uma agregação existente do seu conjunto de instâncias de agregação.
        /// </summary>
        /// <param name="aggregate">Agregação que será removida.</param>
        void Delete(TAggregate aggregate);

        /// <summary>
        /// Informa ao repositório de dados que a instância de uma agregação existente pode ter sido alterada.
        /// </summary>
        /// <param name="aggregate">Agregação que o estado dos dados podem ter sido alterados.</param>
        void Update(TAggregate aggregate);
    }
}
