namespace Abstractor.Cqrs.EntityFramework.Interfaces
{
    /// <summary>
    ///     Abstracts the Entity Framework interal change tracker.
    /// </summary>
    public interface IEntityFrameworkChangeTracker
    {
        /// <summary>
        ///     Detaches all entries from the change tracker.
        /// </summary>
        void Clear();
    }
}