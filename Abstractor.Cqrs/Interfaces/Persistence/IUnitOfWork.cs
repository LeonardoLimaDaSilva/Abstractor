namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    /// Sincroniza as alterações de estado de dados com um repositório de dados.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Efetua as alterações no repositório de dados.
        /// </summary>
        void Commit();
    }
}
