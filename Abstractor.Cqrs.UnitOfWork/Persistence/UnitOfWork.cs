using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.EntityFramework.Extensions;
using Abstractor.Cqrs.EntityFramework.Interfaces;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.UnitOfWork.Persistence
{
    /// <summary>
    ///     Implementation of Unit of Work that synchronizes the contexts of Entity Framework, Azure Blob Storage, Azure Table
    ///     Storage and Azure Queue Storage, ensuring the transactional behaviour between all of them.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IAzureBlobContext _blobContext;
        private readonly IEntityFrameworkContext _entityContext;
        private readonly ILogger _logger;
        private readonly IAzureQueueContext _queueContext;
        private readonly IAzureTableContext _tableContext;

        /// <summary>
        ///     UnitOfWork constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="blobContext"></param>
        /// <param name="tableContext"></param>
        /// <param name="queueContext"></param>
        /// <param name="entityContext"></param>
        public UnitOfWork(
            ILogger logger,
            IAzureBlobContext blobContext,
            IAzureTableContext tableContext,
            IAzureQueueContext queueContext,
            IEntityFrameworkContext entityContext)
        {
            _logger = logger;
            _blobContext = blobContext;
            _tableContext = tableContext;
            _queueContext = queueContext;
            _entityContext = entityContext;
        }

        /// <summary>
        ///     Clears the internal tracking of all contexts.
        /// </summary>
        public void Clear()
        {
            _logger.Log("Clearing Entity Framework context...");
            _entityContext.ChangeTracker().Clear();

            _logger.Log("Clearing Azure Queue context...");
            _queueContext.Clear();

            _logger.Log("Clearing Azure Table context...");
            _tableContext.Clear();

            _logger.Log("Clearing Azure Blob context...");
            _blobContext.Clear();
        }

        /// <summary>
        ///     Commits all the changes ocurred in all contexts, and performs rollback in case of exception.
        /// </summary>
        public void Commit()
        {
            try
            {
                _blobContext.SaveChanges();
                _logger.Log("Azure Blob context commited.");

                _tableContext.SaveChanges();
                _logger.Log("Azure Table context commited.");

                _queueContext.SaveChanges();
                _logger.Log("Azure Queue context commited.");

                _entityContext.SaveChanges();
                _logger.Log("Entity Framework context commited.");
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception caught: {ex.Message}");

                RollbackAll();

                throw;
            }
        }

        /// <summary>
        ///     Undo all performed operations.
        /// </summary>
        private void RollbackAll()
        {
            _logger.Log("Executing Azure Queue context rollback...");
            _queueContext.Rollback();

            _logger.Log("Executing Azure Table context rollback...");
            _tableContext.Rollback();

            _logger.Log("Executing Azure Blob context rollback...");
            _blobContext.Rollback();
        }
    }
}