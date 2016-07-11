namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    /// <summary>
    ///     Stores the necessary information to perform and undo an operation.
    /// </summary>
    public class BaseDataSetOperation
    {
        public object CurrentEntity { get; set; }

        public object PreviousEntity { get; set; }

        public BaseDataSetOperationType Type { get; set; }

        public bool Done { get; set; }
    }
}