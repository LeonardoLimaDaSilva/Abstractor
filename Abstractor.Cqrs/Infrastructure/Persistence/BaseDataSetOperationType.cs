namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     The type of operation to be performed on the persistence mechanism.
    /// </summary>
    public enum BaseDataSetOperationType
    {
        /// <summary>
        ///     Indicates that an entity is going to be inserted.
        /// </summary>
        Insert,

        /// <summary>
        ///     Indicates that an entity is going to be updated.
        /// </summary>
        Update,

        /// <summary>
        ///     Indicates that an entity is going to be deleted.
        /// </summary>
        Delete
    }
}