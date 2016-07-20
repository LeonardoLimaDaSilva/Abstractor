using System;
using System.Data.Entity;
using System.Linq;
using Abstractor.Cqrs.EntityFramework.Interfaces;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.EntityFramework.Persistence
{
    /// <summary>
    ///     Base repository implementation specific for Entity Framework.
    /// </summary>
    /// <typeparam name="TAggregate">Aggregate type.</typeparam>
    internal sealed class EntityFrameworkRepository<TAggregate> : IEntityFrameworkRepository<TAggregate>
        where TAggregate : AggregateRoot
    {
        private readonly Func<DbContext> _contextProvider;

        public EntityFrameworkRepository(Func<DbContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        /// <summary>
        ///     Adds a new aggregate that is being tracked but not exists in database.
        /// </summary>
        /// <param name="aggregate">Aggregate to be added.</param>
        public void Create(TAggregate aggregate)
        {
            Guard.ArgumentIsNotNull(aggregate, nameof(aggregate));

            _contextProvider().Entry(aggregate).State = EntityState.Added;
        }

        /// <summary>
        ///     Marks the existing tracked aggregate for deletion.
        /// </summary>
        /// <param name="aggregate">Aggregate to be deleted.</param>
        public void Delete(TAggregate aggregate)
        {
            Guard.ArgumentIsNotNull(aggregate, nameof(aggregate));

            _contextProvider().Entry(aggregate).State = EntityState.Deleted;
        }

        /// <summary>
        ///     Marks the entire aggregate as modified, allowing the modification of an instance created in a disconnected
        ///     scenario, i.e., using the "new" initializer.
        /// </summary>
        /// <param name="aggregate">Aggregate to be modified.</param>
        public void Update(TAggregate aggregate)
        {
            Guard.ArgumentIsNotNull(aggregate, nameof(aggregate));

            _contextProvider().Entry(aggregate).State = EntityState.Modified;
        }

        /// <summary>
        ///     Returns a queryable representation of the DbSet of the given aggregate type.
        /// </summary>
        /// <returns>Queryable list of aggregates.</returns>
        public IQueryable<TAggregate> Query()
        {
            return _contextProvider().Set<TAggregate>();
        }

        /// <summary>
        ///     Returns a single instance of the aggregate by its primary key.
        /// </summary>
        /// <param name="primaryKey">List of values used to identify an aggregate.</param>
        /// <returns>Single instance of the aggregate.</returns>
        public TAggregate Get(params object[] primaryKey)
        {
            Guard.ArgumentIsNotNull(primaryKey, nameof(primaryKey));

            return _contextProvider().Set<TAggregate>().Find(primaryKey);
        }
    }
}