using System;
using System.Data.Entity;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.SimpleInjector.EntityFramework.Interfaces;

namespace Abstractor.Cqrs.SimpleInjector.EntityFramework.Persistence
{
    internal sealed class EntityFrameworkRepository<TAggregate> : IEntityFrameworkRepository<TAggregate> where TAggregate : AggregateRoot
    {
        private readonly Func<DbContext> _contextProvider;

        public EntityFrameworkRepository(Func<DbContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public void Create(TAggregate entity)
        {
            var context = _contextProvider();
            if (context.Entry(entity).State == EntityState.Detached)
                context.Set<TAggregate>().Add(entity);
        }

        public void Delete(TAggregate entity)
        {
            var context = _contextProvider();
            if (context.Entry(entity).State != EntityState.Deleted)
                context.Set<TAggregate>().Remove(entity);
        }

        public void Update(TAggregate entity)
        {
            var entry = _contextProvider().Entry(entity);
            entry.State = EntityState.Modified;
        }

        public IQueryable<TAggregate> Query()
        {
            return _contextProvider().Set<TAggregate>();
        }

        public TAggregate Get(object primaryKey)
        {
            return _contextProvider().Set<TAggregate>().Find(primaryKey);
        }
    }
}
