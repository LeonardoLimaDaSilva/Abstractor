using System.Data.Entity;
using System.Linq;

namespace Abstractor.Cqrs.EntityFramework.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        ///     Detaches all entries from the change tracker.
        /// </summary>
        /// <param name="dbContext"></param>
        public static void Clear(this DbContext dbContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(entry => entry.Entity != null))
                entry.State = EntityState.Detached;
        }
    }
}