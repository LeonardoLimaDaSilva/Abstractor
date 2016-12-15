using System;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CompositionRoot;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the query handler.
    /// </summary>
    /// <typeparam name="TQuery">Query to be handled.</typeparam>
    /// <typeparam name="TResult">Return type.</typeparam>
    public sealed class QueryAsyncLoggerDecorator<TQuery, TResult> : IQueryAsyncHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly Func<IQueryAsyncHandler<TQuery, TResult>> _handlerFactory;
        private readonly Func<ILogger> _logger;
        private readonly GlobalSettings _settings;
        private readonly ILoggerSerializer _loggerSerializer;
        private readonly IStopwatch _stopwatch;

        public QueryAsyncLoggerDecorator(
            Func<IQueryAsyncHandler<TQuery, TResult>> handlerFactory,
            IAttributeFinder attributeFinder,
            IStopwatch stopwatch,
            ILoggerSerializer loggerSerializer,
            Func<ILogger> logger,
            GlobalSettings settings)
        {
            _handlerFactory = handlerFactory;
            _attributeFinder = attributeFinder;
            _stopwatch = stopwatch;
            _loggerSerializer = loggerSerializer;
            _logger = logger;
            _settings = settings;
        }

        /// <summary>
        ///     Logs the execution of the query handler.
        /// </summary>
        /// <param name="query">Query to be handled.</param>
        /// <returns>Returns an Task of type <typeparamref name="TResult" />.</returns>
        public Task<TResult> HandleAsync(TQuery query)
        {
            var handler = _handlerFactory();

            if (!_attributeFinder.Decorates(query.GetType(), typeof (LogAttribute)) && !_settings.EnableLogging)
                return handler.HandleAsync(query);

            _stopwatch.Start();

            var logger = _logger();

            try
            {
                logger.Log($"Executing query \"{query.GetType().Name}\" with the parameters:");

                try
                {
                    var parameters = _loggerSerializer.Serialize(query);
                    logger.Log(parameters);
                }
                catch (Exception ex)
                {
                    logger.Log($"Could not serialize the parameters: {ex.Message}");
                }

                return handler.HandleAsync(query);
            }
            catch (Exception ex)
            {
                logger.Log("Exception caught: " + ex.Message);

                if (ex.InnerException != null)
                    logger.Log("Inner exception caught: " + ex.InnerException.Message);

                throw;
            }
            finally
            {
                _stopwatch.Stop();

                logger.Log($"Query \"{query.GetType().Name}\" executed in {_stopwatch.GetElapsed()}.");
            }
        }
    }
}