using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    public sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly IContainer _container;

        public QueryDispatcher(IContainer container)
        {
            _container = container;
        }

        [DebuggerStepThrough]
        public TResult Dispatch<TResult>(IQuery<TResult> query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _container.GetInstance(handlerType);

            return handler.Handle((dynamic)query);
        }

        [DebuggerStepThrough]
        public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var handlerType = typeof(IQueryAsyncHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _container.GetInstance(handlerType);

            return await handler.HandleAsync((dynamic)query);
        }
    }
}
