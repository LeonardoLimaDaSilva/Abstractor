using System;
using System.Configuration;
using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    /// <summary>
    ///     Overrides the hook necessary for constructing the specific Azure Queue data set.
    /// </summary>
    public class AzureQueueContext : BaseDataContext
    {
        private readonly string _connectionString;

        public AzureQueueContext()
        {
        }

        public AzureQueueContext(string connectionName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }

        /// <summary>
        ///     Gets an instance of an AzureQueueSet of given <see cref="TEntity" /> type.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>Data set abstraction.</returns>
        protected override IBaseDataSet GetDataSet<TEntity>()
        {
            var genericType = typeof (AzureQueueSet<>).MakeGenericType(typeof (TEntity));
            return (IBaseDataSet) Activator.CreateInstance(genericType, _connectionString);
        }
    }
}