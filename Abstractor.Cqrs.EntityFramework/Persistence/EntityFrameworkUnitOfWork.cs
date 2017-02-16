using System;
using System.Data.Entity.Validation;
using System.Linq;
using Abstractor.Cqrs.EntityFramework.Extensions;
using Abstractor.Cqrs.EntityFramework.Interfaces;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.EntityFramework.Persistence
{
    /// <summary>
    ///     Wraps the default DbContext Unit of Work for providing logging and a clearing method.
    /// </summary>
    internal sealed class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly IEntityFrameworkContext _context;
        private readonly ILogger _logger;

        public EntityFrameworkUnitOfWork(
            IEntityFrameworkContext context,
            ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        ///     Detaches all entries from the DbContext change tracker.
        /// </summary>
        public void Clear()
        {
            _context.ChangeTracker().Clear();
        }

        /// <summary>
        ///     Commit the changes tracked by DbContext.
        /// </summary>
        public void Commit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                _logger.Log("Entity validation errors:");

                foreach (var ve in ex.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
                    _logger.Log($"-- {ve.PropertyName}: {ve.ErrorMessage}");

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