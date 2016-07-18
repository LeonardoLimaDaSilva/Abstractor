namespace Abstractor.Cqrs.Interfaces.Persistence
{
    /// <summary>
    ///     Synchronizes entities state changes to a data persistence mechanism.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        ///     Commits the changes.
        /// </summary>
        void Commit();

        /// <summary>
        ///     Clears all tracked states.
        /// </summary>
        void Clear();
    }
}