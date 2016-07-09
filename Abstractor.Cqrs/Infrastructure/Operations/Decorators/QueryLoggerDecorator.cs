using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the query handler.
    /// </summary>
    /// <typeparam name="TQuery">Query to be handled.</typeparam>
    /// <typeparam name="TResult">Return type.</typeparam>
    [DebuggerStepThrough]
    public sealed class QueryLoggerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly Func<IQueryHandler<TQuery, TResult>> _handlerFactory;
        private readonly IStopwatch _stopwatch;
        private readonly ILoggerSerializer _loggerSerializer;
        private readonly ILogger _logger;

        public QueryLoggerDecorator(
            Func<IQueryHandler<TQuery, TResult>> handlerFactory,
            IStopwatch stopwatch,
            ILoggerSerializer loggerSerializer,
            ILogger logger)
        {
            _handlerFactory = handlerFactory;
            _stopwatch = stopwatch;
            _loggerSerializer = loggerSerializer;
            _logger = logger;
        }

        /// <summary>
        ///     Logs the execution of the query handler.
        /// </summary>
        /// <param name="query">Query to be handled.</param>
        /// <returns>Returns an object of type <typeparamref name="TResult" />.</returns>
        public TResult Handle(TQuery query)
        {
            _stopwatch.Start();

            try
            {
                _logger.Log($"Executing query \"{query.GetType().Name}\" with the parameters:");

                try
                {
                    var parameters = _loggerSerializer.Serialize(query);
                    _logger.Log(parameters);
                }
                catch (Exception ex)
                {
                    _logger.Log($"Could not serialize the parameters: {ex.Message}");
                }

                return _handlerFactory().Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Log("Exception caught: " + ex.Message);

                if (ex.InnerException != null)
                    _logger.Log("Inner exception caught: " + ex.InnerException.Message);

                throw;
            }
            finally
            {
                _stopwatch.Stop();

                _logger.Log($"Query \"{query.GetType().Name}\" executed in {_stopwatch.GetElapsed()}.");
            }
        }
    }
}