namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     Stores the necessary information to perform and undo an operation.
    /// </summary>
    public class BaseDataSetOperation
    {
        /// <summary>
        ///     The current entity being handled by the operation.
        /// </summary>
        public object CurrentEntity { get; set; }

        /// <summary>
        ///     Indicates whether the operation has completed.
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        ///     The previous entity being handled by the operation.
        /// </summary>
        public object PreviousEntity { get; set; }

        /// <summary>
        ///     Type of operation.
        /// </summary>
        public BaseDataSetOperationType Type { get; set; }
    }
}