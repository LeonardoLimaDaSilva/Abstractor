namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     The type of operation to be performed on the persistence mechanism.
    /// </summary>
    public enum BaseDataSetOperationType
    {
        Insert,
        Update,
        Delete
    }
}