using System.Data.Entity;
using System.Linq;
using Abstractor.Cqrs.EntityFramework.Interfaces;

namespace Abstractor.Cqrs.EntityFramework.Persistence
{
    /// <summary>
    ///     Encapsulates the clear method of the <see cref="IEntityFrameworkContext"/> extension.
    /// </summary>
    internal class EntityFrameworkChangeTracker : IEntityFrameworkChangeTracker
    {
        private readonly IEntityFrameworkContext _efContext;

        public EntityFrameworkChangeTracker(IEntityFrameworkContext efContext)
        {
            _efContext = efContext;
        }

        /// <summary>
        ///     Detaches all entries from the change tracker.
        /// </summary>
        public void Clear()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var dbContext = _efContext as DbContext;
            if (dbContext == null) return;

            foreach (var entry in dbContext.ChangeTracker.Entries().Where(entry => entry.Entity != null))
                entry.State = EntityState.Detached;
        }
    }
}