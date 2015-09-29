using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    /// Garante que o <see cref="IQueryHandler{TQuery, TResult}"/> receba apenas parâmetros não nulos.
    /// </summary>
    /// <typeparam name="TQuery">Consulta que será executada.</typeparam>
    /// <typeparam name="TResult">Retorno da consulta.</typeparam>
    public sealed class QueryNotNullDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult> 
        where TQuery : IQuery<TResult>
    {
        private readonly Func<IQueryHandler<TQuery, TResult>> _handlerFactory;

        public QueryNotNullDecorator(Func<IQueryHandler<TQuery, TResult>> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        [DebuggerStepThrough]
        public TResult Handle(TQuery query)
        {
            if (Equals(query, null)) throw new ArgumentNullException(nameof(query));
            return _handlerFactory().Handle(query);
        }
    }
}