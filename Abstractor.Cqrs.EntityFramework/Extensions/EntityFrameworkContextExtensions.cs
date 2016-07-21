using System.Data.Entity;
using System.Linq;
using Abstractor.Cqrs.EntityFramework.Interfaces;

namespace Abstractor.Cqrs.EntityFramework.Extensions
{
    public static class EntityFrameworkContextExtensions
    {
        /// <summary>
        ///     Detaches all entries from the change tracker.
        /// </summary>
        /// <param name="efContext">DbContext abstraction.</param>
        public static void Clear(this IEntityFrameworkContext efContext)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            foreach (var entry in ((DbContext)efContext).ChangeTracker.Entries().Where(entry => entry.Entity != null))
                entry.State = EntityState.Detached;
        }
    }
}