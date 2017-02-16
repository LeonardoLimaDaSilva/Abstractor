using System;
using System.Configuration;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    /// <summary>
    ///     Overrides the hook necessary for constructing the specific Azure Queue data set.
    /// </summary>
    public class AzureQueueContext : BaseDataContext, IAzureQueueContext
    {
        private readonly string _connectionString;

        /// <summary>
        ///     AzureQueueContext constructor.
        /// </summary>
        /// <param name="connectionName">Name of Azure connection.</param>
        public AzureQueueContext(string connectionName)
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
            var genericType = typeof(AzureQueueSet<>).MakeGenericType(typeof(TEntity));
            return (IBaseDataSet) Activator.CreateInstance(genericType, _connectionString);
        }
    }
}