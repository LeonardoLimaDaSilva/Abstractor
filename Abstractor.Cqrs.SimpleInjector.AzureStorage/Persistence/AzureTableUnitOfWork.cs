using System;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Persistence
{
    public sealed class AzureTableUnitOfWork : IUnitOfWork
    {
        private readonly Func<TsContext> _contextProvider;
        private readonly ILogger _logger;

        public AzureTableUnitOfWork(
            Func<TsContext> contextProvider,
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
            catch (Exception e)
            {
                _logger.Log(e.Message);
                throw;
            }
        }
    }
}
