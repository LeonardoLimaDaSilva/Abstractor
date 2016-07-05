using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.EntityFramework.Persistence
{
    internal sealed class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly Func<DbContext> _contextProvider;
        private readonly ILogger _logger;

        public EntityFrameworkUnitOfWork(
            Func<DbContext> contextProvider,
            ILogger logger)
        {
            _contextProvider = contextProvider;
            _logger = logger;
        }

        public void Commit()
        {
            try
            {
                _contextProvider().SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                _logger.Log(ex.EntityValidationErrors
                              .SelectMany(eve => eve.ValidationErrors)
                              .Aggregate("", (c, ve) => c + $"{ve.PropertyName}: {ve.ErrorMessage}"));

                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);

                throw;
            }
        }
    }
}