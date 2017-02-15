using System;
using System.Configuration;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.Infrastructure.Persistence;

namespace Abstractor.Cqrs.AzureStorage.Blob
{
    /// <summary>
    ///     Overrides the hook necessary for constructing the specific Azure Blob data set.
    /// </summary>
    public class AzureBlobContext : BaseDataContext, IAzureBlobContext
    {
        private readonly string _connectionString;

        /// <summary>
        ///     AzureBlobContext constructor.
        /// </summary>
        /// <param name="connectionName">Name of Azure connection.</param>
        public AzureBlobContext(string connectionName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }

        /// <summary>
        ///     Gets an instance of an AzureBlobSet of given <see cref="TEntity" /> type.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>Data set abstraction.</returns>
        protected override IBaseDataSet GetDataSet<TEntity>()
        {
            var genericType = typeof(AzureBlobSet<>).MakeGenericType(typeof(TEntity));
            return (IBaseDataSet) Activator.CreateInstance(genericType, _connectionString);
        }
    }
}