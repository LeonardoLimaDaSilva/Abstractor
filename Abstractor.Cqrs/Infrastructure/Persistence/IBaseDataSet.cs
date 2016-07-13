namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     Represents the base data set that stores the write and read operations and abstracts the commit and rollback
    ///     algorithm.
    /// </summary>
    public interface IBaseDataSet
    {
        /// <summary>
        ///     Commits all pending operations.
        /// </summary>
        void Commit();

        /// <summary>
        ///     Undoes all done operations.
        /// </summary>
        void Rollback();
    }
}