using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    /// Garante que o <see cref="IQueryAsyncHandler{TQuery, TResult}"/> receba apenas parâmetros não nulos.
    /// </summary>
    /// <typeparam name="TQuery">Consulta que será executada.</typeparam>
    /// <typeparam name="TResult">Retorno da consulta.</typeparam>
    public sealed class QueryAsyncNotNullDecorator<TQuery, TResult> : IQueryAsyncHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly Func<IQueryAsyncHandler<TQuery, TResult>> _handlerFactory;

        public QueryAsyncNotNullDecorator(Func<IQueryAsyncHandler<TQuery, TResult>> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        [DebuggerStepThrough]
        public Task<TResult> HandleAsync(TQuery query)
        {
            if (Equals(query, null)) throw new ArgumentNullException(nameof(query));
            return _handlerFactory().HandleAsync(query);
        }
    }
}