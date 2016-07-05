using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Inicia um novo escopo do ciclo de vida da consulta assíncrona, caso não exista nenhum.
    /// </summary>
    /// <typeparam name="TQuery">Consulta que será executada.</typeparam>
    /// <typeparam name="TResult">Retorno da consulta.</typeparam>
    [DebuggerStepThrough]
    public sealed class QueryAsyncLifetimeScopeDecorator<TQuery, TResult> : IQueryAsyncHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IContainer _container;
        private readonly Func<IQueryAsyncHandler<TQuery, TResult>> _handlerFactory;

        public QueryAsyncLifetimeScopeDecorator(
            IContainer container,
            Func<IQueryAsyncHandler<TQuery, TResult>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Inicia um novo escopo do ciclo de vida antes de executar a consulta assíncrona.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<TResult> HandleAsync(TQuery query)
        {
            if (_container.GetCurrentLifetimeScope() != null)
                return _handlerFactory().HandleAsync(query);

            using (_container.BeginLifetimeScope())
                return _handlerFactory().HandleAsync(query);
        }
    }
}