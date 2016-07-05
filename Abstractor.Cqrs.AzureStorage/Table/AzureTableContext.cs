using System;
using System.Configuration;
using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.AzureStorage.Table
{
    public class AzureTableContext : BaseDataContext
    {
        private readonly string _connectionString;

        public AzureTableContext(string connectionName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }

        protected override IBaseDataSet GetDataSet<TEntity>()
        {
            var genericType = typeof (AzureTableSet<>).MakeGenericType(typeof (TEntity));
            return (IBaseDataSet) Activator.CreateInstance(genericType, _connectionString);
        }
    }
}