using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Abstractor.Cqrs.AzureStorage.Blob;
using Abstractor.Cqrs.AzureStorage.Queue;
using Abstractor.Cqrs.AzureStorage.Table;
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
        private readonly Func<AzureBlobContext> _blobContext;
        private readonly Func<DbContext> _efContext;
        private readonly ILogger _logger;
        private readonly Func<AzureQueueContext> _queueContext;
        private readonly Func<AzureTableContext> _tableContext;

        public UnitOfWork(
            ILogger logger,
            Func<AzureBlobContext> blobContext,
            Func<AzureTableContext> tableContext,
            Func<AzureQueueContext> queueContext,
            Func<DbContext> efContext)
        {
            _logger = logger;
            _blobContext = blobContext;
            _tableContext = tableContext;
            _queueContext = queueContext;
            _efContext = efContext;
        }

        /// <summary>
        ///     Commits all the changes ocurred in all contexts, and performs rollback in case of exception.
        /// </summary>
        public void Commit()
        {
            try
            {
                _blobContext().SaveChanges();
                _logger.Log("Azure Blob context commited.");

                _tableContext().SaveChanges();
                _logger.Log("Azure Table context commited.");

                _queueContext().SaveChanges();
                _logger.Log("Azure Queue context commited.");

                _efContext().SaveChanges();
                _logger.Log("Entity Framework context commited.");
            }
            catch (DbEntityValidationException ex)
            {
                _logger.Log($"Entity Framework validation exception caught: {ex.Message}");
                _logger.Log(ex.EntityValidationErrors
                              .SelectMany(eve => eve.ValidationErrors)
                              .Aggregate(string.Empty, (c, ve) => c + $"{ve.PropertyName}: {ve.ErrorMessage}"));

                RollbackAll();

                throw;
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception caught: {ex.Message}");
                _logger.Log(ex.Message);

                RollbackAll();

                throw;
            }
        }

        /// <summary>
        ///     Clears the internal tracking of all contexts.
        /// </summary>
        public void Clear()
        {
            _logger.Log("Clearing Entity Framework context...");
            _efContext().Clear();

            _logger.Log("Clearing Azure Queue context...");
            _queueContext().Clear();

            _logger.Log("Clearing Azure Table context...");
            _tableContext().Clear();

            _logger.Log("Clearing Azure Blob context...");
            _blobContext().Clear();
        }

        /// <summary>
        ///     Undo all performed operations.
        /// </summary>
        private void RollbackAll()
        {
            _logger.Log("Executing Azure Queue context rollback...");
            _queueContext().Rollback();

            _logger.Log("Executing Azure Table context rollback...");
            _tableContext().Rollback();

            _logger.Log("Executing Azure Blob context rollback...");
            _blobContext().Rollback();
        }
    }
}