namespace Abstractor.Cqrs.EntityFramework.Interfaces
{
    /// <summary>
    ///     Abstraction of a context specific for Entity Framework.
    /// </summary>
    public interface IEntityFrameworkContext
    {
        /// <summary>
        ///     Commits the changes of all the stored data sets operations.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}