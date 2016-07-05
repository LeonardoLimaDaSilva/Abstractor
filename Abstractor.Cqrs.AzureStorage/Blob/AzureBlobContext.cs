using System;
using System.Configuration;
using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    public class AzureBlobContext : BaseDataContext
    {
        private readonly string _connectionString;

        public AzureBlobContext(string connectionName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }

        protected override IBaseDataSet GetDataSet<TEntity>()
        {
            var genericType = typeof (AzureBlobSet<>).MakeGenericType(typeof (TEntity));
            return (IBaseDataSet) Activator.CreateInstance(genericType, _connectionString);
        }
    }
}