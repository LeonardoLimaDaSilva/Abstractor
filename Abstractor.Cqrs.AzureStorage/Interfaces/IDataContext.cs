namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Abstraction of a data context.
    /// </summary>
    public interface IDataContext
    {
        /// <summary>
        ///     Clears the internal context.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Undoes the changes of all the stored data sets operations.
        /// </summary>
        void Rollback();

        /// <summary>
        ///     Commits the changes of all the stored data sets operations.
        /// </summary>
        void SaveChanges();
    }
}