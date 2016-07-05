using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    /// <summary>
    ///     Processador de consultas.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly IContainer _container;

        public QueryDispatcher(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     Dispara um <see cref="IQueryHandler{TQuery,TResult}" /> registrado em <see cref="IContainer" />.
        /// </summary>
        /// <typeparam name="TResult">Tipo do resultado da consulta.</typeparam>
        /// <param name="query">Objeto de consulta.</param>
        /// <returns>Resultado da consulta.</returns>
        public TResult Dispatch<TResult>(IQuery<TResult> query)
        {
            Guard.ArgumentIsNotNull(query, nameof(query));

            var handlerType = typeof (IQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResult));
            dynamic handler = _container.GetInstance(handlerType);

            return handler.Handle((dynamic) query);
        }

        /// <summary>
        ///     Dispara, de forma assíncrona, um <see cref="IQueryHandler{TQuery,TResult}" /> registrado em
        ///     <see cref="IContainer" />.
        /// </summary>
        /// <typeparam name="TResult">Tipo do resultado da consulta.</typeparam>
        /// <param name="query">Objeto de consulta.</param>
        /// <returns>Task assíncrona contendo o resultado da consulta.</returns>
        public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query)
        {
            Guard.ArgumentIsNotNull(query, nameof(query));

            var handlerType = typeof (IQueryAsyncHandler<,>).MakeGenericType(query.GetType(), typeof (TResult));
            dynamic handler = _container.GetInstance(handlerType);

            return await handler.HandleAsync((dynamic) query);
        }
    }
}