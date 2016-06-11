using System;
using System.Collections.Generic;
using System.Configuration;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Persistence
{
    public class TsContext : IDisposable
    {
        private readonly List<IUnitOfWork> _internalContext;
        private readonly string _connectionString;

        public TsContext()
        {
        }

        public TsContext(string connectionName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
            _internalContext = new List<IUnitOfWork>();
        }

        public TableSet<TEntity> Set<TEntity>()
            where TEntity : class, new()
        {
            if (_connectionString == null) throw new ArgumentNullException(nameof(_connectionString));

            var set = new TableSet<TEntity>(_connectionString);

            _internalContext.Add(set);

            return set;
        }

        public void SaveChanges()
        {
            foreach (var context in _internalContext)
                context.Commit();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // if (!disposing) return;
            //TODO ???
        }
    }
}