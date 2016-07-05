namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    public class BaseDataSetOperation
    {
        public object Entity { get; set; }

        public object OldEntity { get; set; }

        public BaseDataSetOperationType Type { get; set; }

        public bool Done { get; set; }
    }
}