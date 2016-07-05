using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Inicia um novo escopo do ciclo de vida da consulta, caso não exista nenhum.
    /// </summary>
    /// <typeparam name="TQuery">Consulta que será executada.</typeparam>
    /// <typeparam name="TResult">Retorno da consulta.</typeparam>
    [DebuggerStepThrough]
    public sealed class QueryLifetimeScopeDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IContainer _container;
        private readonly Func<IQueryHandler<TQuery, TResult>> _handlerFactory;

        public QueryLifetimeScopeDecorator(
            IContainer container,
            Func<IQueryHandler<TQuery, TResult>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Inicia um novo escopo do ciclo de vida antes de executar a consulta.
        /// </summary>
        /// <param name="query">Objeto de consulta.</param>
        /// <returns></returns>
        public TResult Handle(TQuery query)
        {
            if (_container.GetCurrentLifetimeScope() != null)
                return _handlerFactory().Handle(query);

            using (_container.BeginLifetimeScope())
                return _handlerFactory().Handle(query);
        }
    }
}