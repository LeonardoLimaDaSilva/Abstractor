using System;
using Abstractor.Cqrs.EntityFramework.Interfaces;
using Abstractor.Cqrs.EntityFramework.Persistence;

namespace Abstractor.Cqrs.EntityFramework.Extensions
{
    /// <summary>
    ///     Provides an amenable way for mocking the extension method.
    /// </summary>
    public static class EntityFrameworkContextExtensions
    {
        /// <summary>
        ///     Delegate factory for the EntityFrameworkChangeTracker.
        /// </summary>
        public static Func<IEntityFrameworkContext, IEntityFrameworkChangeTracker> Factory { get; set; }

        static EntityFrameworkContextExtensions()
        {
            Factory = x => new EntityFrameworkChangeTracker(x);
        }

        /// <summary>
        ///     Constructs a new instance of the change tracker.
        /// </summary>
        /// <param name="efContext">Entity Framework context.</param>
        /// <returns></returns>
        public static IEntityFrameworkChangeTracker ChangeTracker(this IEntityFrameworkContext efContext)
        {
            return Factory(efContext);
        }
    }
}