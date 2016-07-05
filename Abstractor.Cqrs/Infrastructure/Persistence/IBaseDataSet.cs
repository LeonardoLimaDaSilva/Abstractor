namespace Abstractor.Cqrs.Infrastructure.Persistence
{
    public interface IBaseDataSet
    {
        void Commit();
        void Rollback();
    }
}