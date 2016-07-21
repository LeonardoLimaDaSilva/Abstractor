using System;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Ensures that there is a lifetime scope before the application event execution.
    /// </summary>
    /// <typeparam name="TEvent">Application event in which the handler subscribes to.</typeparam>
    public sealed class ApplicationEventLifetimeScopeDecorator<TEvent> : IApplicationEventHandler<TEvent>
        where TEvent : IApplicationEvent
    {
        private readonly IContainer _container;
        private readonly Func<IApplicationEventHandler<TEvent>> _handlerFactory;

        public ApplicationEventLifetimeScopeDecorator(
            IContainer container,
            Func<IApplicationEventHandler<TEvent>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Ensures that there is a lifetime scope before the application event execution.
        /// </summary>
        /// <param name="applicationEvent">Application event in which the handler subscribes to.</param>
        public void Handle(TEvent applicationEvent)
        {
            if (_container.GetCurrentLifetimeScope() != null)
            {
                _handlerFactory().Handle((dynamic)applicationEvent);
                return;
            }

            using (_container.BeginLifetimeScope())
                _handlerFactory().Handle((dynamic)applicationEvent);
        }
    }
}