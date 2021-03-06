using System;
using System.Configuration;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.AzureStorage.Table
{
    /// <summary>
    ///     Overrides the hook necessary for constructing the specific Azure Table data set.
    /// </summary>
    public class AzureTableContext : BaseDataContext, IAzureTableContext
    {
        private readonly string _connectionString;

        /// <summary>
        ///     AzureTableContext constructor.
        /// </summary>
        /// <param name="connectionName">Name of Azure connection.</param>
        public AzureTableContext(string connectionName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }

        /// <summary>
        ///     Gets an instance of an AzureQueueSet of given TEntity type.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>Data set abstraction.</returns>
        protected override IBaseDataSet GetDataSet<TEntity>()
        {
            var genericType = typeof(AzureTableSet<>).MakeGenericType(typeof(TEntity));
            return (IBaseDataSet) Activator.CreateInstance(genericType, _connectionString);
        }
    }
}