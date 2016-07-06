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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILogger _logger;
        private readonly Func<AzureBlobContext> _blobContext;
        private readonly Func<AzureTableContext> _tableContext;
        private readonly Func<AzureQueueContext> _queueContext;
        private readonly Func<DbContext> _efContext;

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

        public void Commit()
        {
            try
            {
                _blobContext().SaveChanges();
                _tableContext().SaveChanges();
                _queueContext().SaveChanges();
                _efContext().SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                _logger.Log(ex.EntityValidationErrors
                              .SelectMany(eve => eve.ValidationErrors)
                              .Aggregate(string.Empty, (c, ve) => c + $"{ve.PropertyName}: {ve.ErrorMessage}"));

                RollbackAll();

                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);

                RollbackAll();

                throw;
            }
        }

        private void RollbackAll()
        {
            _logger.Log("Executing AzureQueueContext rollback...");
            _queueContext().Rollback();

            _logger.Log("Executing AzureTableContext rollback...");
            _tableContext().Rollback();

            _logger.Log("Executing AzureBlobContext rollback...");
            _blobContext().Rollback();
        }
    }
}
