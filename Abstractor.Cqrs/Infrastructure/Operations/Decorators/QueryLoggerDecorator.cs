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
        private readonly ILogger _logger;

        public QueryLoggerDecorator(
            Func<IQueryHandler<TQuery, TResult>> handlerFactory,
            ILogger logger)
        {
            _handlerFactory = handlerFactory;
            _logger = logger;
        }

        /// <summary>
        ///     Logs the execution of the query handler.
        /// </summary>
        /// <param name="query">Query to be handled.</param>
        /// <returns>Returns an object of type <typeparamref name="TResult" />.</returns>
        public TResult Handle(TQuery query)
        {
            var sw = Stopwatch.StartNew();
            sw.Start();

            var handler = _handlerFactory();

            try
            {
                _logger.Log($"Executing query {handler.GetType().Name} with the parameters:");

                try
                {
                    _logger.Log(JsonConvert.SerializeObject(query, Formatting.Indented));
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
                sw.Stop();

                _logger.Log($"Query \"{handler.GetType().Name}\" executed in {sw.Elapsed}.");
            }
        }
    }
}