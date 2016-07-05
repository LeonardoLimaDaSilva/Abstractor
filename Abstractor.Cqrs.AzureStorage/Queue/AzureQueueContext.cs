using System;
using System.Configuration;
using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    public class AzureQueueContext : BaseDataContext
    {
        private readonly string _connectionString;

        public AzureQueueContext(string connectionName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }

        protected override IBaseDataSet GetDataSet<TEntity>()
        {
            var genericType = typeof (AzureQueueSet<>).MakeGenericType(typeof (TEntity));
            return (IBaseDataSet) Activator.CreateInstance(genericType, _connectionString);
        }
    }
}