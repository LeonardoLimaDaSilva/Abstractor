using System;
using System.Data.Entity;
using System.Linq;
using Abstractor.Cqrs.EntityFramework.Interfaces;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.EntityFramework.Persistence
{
    internal sealed class EntityFrameworkRepository<TAggregate> : IEntityFrameworkRepository<TAggregate>
        where TAggregate : AggregateRoot
    {
        private readonly Func<DbContext> _contextProvider;

        public EntityFrameworkRepository(Func<DbContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public void Create(TAggregate entity)
        {
            Guard.ArgumentIsNotNull(entity, nameof(entity));

            _contextProvider().Entry(entity).State = EntityState.Added;
        }

        public void Delete(TAggregate entity)
        {
            Guard.ArgumentIsNotNull(entity, nameof(entity));

            _contextProvider().Entry(entity).State = EntityState.Deleted;
        }

        public void Update(TAggregate entity)
        {
            Guard.ArgumentIsNotNull(entity, nameof(entity));

            _contextProvider().Entry(entity).State = EntityState.Modified;
        }

        public IQueryable<TAggregate> Query()
        {
            return _contextProvider().Set<TAggregate>();
        }

        public TAggregate Get(params object[] primaryKey)
        {
            Guard.ArgumentIsNotNull(primaryKey, nameof(primaryKey));

            return _contextProvider().Set<TAggregate>().Find(primaryKey);
        }
    }
}