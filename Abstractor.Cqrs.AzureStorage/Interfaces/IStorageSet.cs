namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    internal interface IStorageSet
    {
        void Commit();
        void Rollback();
    }
}