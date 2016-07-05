namespace Abstractor.Cqrs.Interfaces.Domain
{
    /// <summary>
    ///     Especifica que uma entidade pode ter seu estado interno restaurado.
    /// </summary>
    /// <typeparam name="T">Tipo da implementação memento.</typeparam>
    public interface IRestorable<T> where T : IMemento
    {
        /// <summary>
        ///     Altera o estado interno de uma entidade através do <paramref name="memento" />.
        /// </summary>
        /// <param name="memento">Representa o estado interno da entidade.</param>
        void Restore(T memento);

        /// <summary>
        ///     Retorna o memento que representa o estado interno da entidade.
        /// </summary>
        /// <returns></returns>
        T Snapshot();
    }
}