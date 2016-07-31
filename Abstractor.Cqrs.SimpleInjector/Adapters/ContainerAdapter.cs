using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.Adapters
{
    /// <summary>
    ///     Simple Injector's implementation of an inversion of control container.
    /// </summary>
    public sealed class ContainerAdapter : IContainer
    {
        private readonly Container _container;

        public ContainerAdapter(Container container)
        {
            _container = container;
            _container.RegisterSingleton(typeof(IContainer), this);
        }

        /// <summary>
        ///     Gets and instance of the given type.
        /// </summary>
        /// <param name="type">Type of instance.</param>
        /// <returns>Instance of given type.</returns>
        public object GetInstance(Type type)
        {
            return _container.GetInstance(type);
        }

        /// <summary>
        ///     Gets all instances registered for the given type.
        /// </summary>
        /// <param name="type">Type of instances.</param>
        /// <returns>Instances registered for given type.</returns>
        public IEnumerable<object> GetAllInstances(Type type)
        {
            return _container.GetAllInstances(type);
        }

        /// <summary>
        ///     Gets the current scope or null if there is none.
        /// </summary>
        /// <returns>Current scope or null.</returns>
        public object GetCurrentLifetimeScope()
        {
            return _container.GetCurrentLifetimeScope();
        }

        /// <summary>
        ///     Begins a new scope for the given container on the current thread.
        /// </summary>
        /// <returns>New scope.</returns>
        public IDisposable BeginLifetimeScope()
        {
            return _container.BeginLifetimeScope();
        }

        /// <summary>
        ///     Registers that an instance of type <see cref="TImplementation" /> will be returned when an instance of type
        ///     <see cref="TService" /> is requested. Uses the container's configured scope lifetime.
        /// </summary>
        /// <typeparam name="TService">Type of abstraction.</typeparam>
        /// <typeparam name="TImplementation">Type of the implementation.</typeparam>
        public void RegisterScoped<TService, TImplementation>()
        {
            _container.Register(typeof(TService), typeof(TImplementation), Lifestyle.Scoped);
        }

        /// <summary>
        ///     Registers that an instance of type <see cref="serviceType" /> will be returned when an instance of type
        ///     <see cref="implementationType" /> is requested. Uses the container's configured scope lifetime.
        /// </summary>
        /// <param name="serviceType">Type of the abstraction.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void RegisterScoped(Type serviceType, Type implementationType)
        {
            _container.Register(serviceType, implementationType, Lifestyle.Scoped);
        }

        /// <summary>
        ///     Registers that a new instance of type <see cref="serviceType" /> will be returned each time that an instance of
        ///     type <see cref="implementationType" /> is requested.
        /// </summary>
        /// <param name="serviceType">Type of the abstraction.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void RegisterTransient(Type serviceType, Type implementationType)
        {
            _container.Register(serviceType, implementationType, Lifestyle.Transient);
        }

        /// <summary>
        ///     Registers all concrete types contained in the assemblies that implements an open generic abstraction, and returns a
        ///     new instance each time an instance of <see cref="openGenericServiceType" /> is requested.
        /// </summary>
        /// <param name="openGenericServiceType">Type of the abstraction.</param>
        /// <param name="assemblies">Assemblies that contains the concrete type</param>
        public void RegisterTransient(Type openGenericServiceType, IEnumerable<Assembly> assemblies)
        {
            _container.Register(openGenericServiceType, assemblies, Lifestyle.Transient);
        }

        /// <summary>
        ///     Registers all concrete types that implements an open generic abstraction as a collection, and returns them as new
        ///     instances each time it's requested.
        /// </summary>
        /// <param name="openGenericServiceType">Type of the abstraction.</param>
        /// <param name="assemblies">Assemblies that contains the concrete types.</param>
        public void RegisterCollection(Type openGenericServiceType, IEnumerable<Assembly> assemblies)
        {
            _container.RegisterCollection(openGenericServiceType, assemblies);
        }

        /// <summary>
        ///     Registers that an instance of type <see cref="TImplementation" /> will return the same instance of type
        ///     <see cref="TService" /> when requested.
        /// </summary>
        /// <typeparam name="TService">Type of abstraction.</typeparam>
        /// <typeparam name="TImplementation">Type of the implementation.</typeparam>
        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register(typeof(TService), typeof(TImplementation), Lifestyle.Singleton);
        }

        /// <summary>
        ///     Ensures that the a new instance of the supplied <see cref="decoratorType" /> is returned, wrapping the original
        ///     <see cref="serviceType" />.
        /// </summary>
        /// <param name="serviceType">Original type.</param>
        /// <param name="decoratorType">Type that wraps the original type.</param>
        public void RegisterDecoratorTransient(Type serviceType, Type decoratorType)
        {
            _container.RegisterDecorator(serviceType, decoratorType, Lifestyle.Transient);
        }

        /// <summary>
        ///     Ensures that the same instance of the supplied <see cref="decoratorType" /> is returned, wrapping the original
        ///     <see cref="serviceType" />.
        /// </summary>
        /// <param name="serviceType">Original type.</param>
        /// <param name="decoratorType">Type that wraps the original type.</param>
        public void RegisterDecoratorSingleton(Type serviceType, Type decoratorType)
        {
            _container.RegisterDecorator(serviceType, decoratorType, Lifestyle.Singleton);
        }

        /// <summary>
        ///     Provides functionality for the deferred resolving of unregistered types.
        /// </summary>
        /// <typeparam name="TService">Type of abstraction.</typeparam>
        /// <typeparam name="TImplementation">Type of the implementation.</typeparam>
        public void RegisterLazySingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            var registration = new Lazy<Registration>(
                () => Lifestyle.Singleton
                    .CreateRegistration<TService, TImplementation>(_container));

            _container.ResolveUnregisteredType += (sender, e) =>
            {
                if (e.UnregisteredServiceType == typeof(TService))
                    e.Register(registration.Value);
            };
        }

        /// <summary>
        ///     Provides functionality for the deferred resolving of unregistered types.
        /// </summary>
        /// <typeparam name="TService">Type of abstraction.</typeparam>
        /// <typeparam name="TImplementation">Type of the implementation.</typeparam>
        public void RegisterLazyScoped<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            var registration = new Lazy<Registration>(
                () => Lifestyle.Scoped
                    .CreateRegistration<TService, TImplementation>(_container));

            _container.ResolveUnregisteredType += (sender, e) =>
            {
                if (e.UnregisteredServiceType == typeof(TService))
                    e.Register(registration.Value);
            };
        }

        /// <summary>
        ///     Provides functionality for resolving delegates of type <see cref="Func{T}" />.
        /// </summary>
        public void AllowResolvingFuncFactories()
        {
            _container.ResolveUnregisteredType += (s, e) =>
            {
                var type = e.UnregisteredServiceType;

                if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Func<>)) return;

                var serviceType = type.GetGenericArguments().First();
                var registration = _container.GetRegistration(serviceType, true);
                var funcType = typeof(Func<>).MakeGenericType(serviceType);

                // Constructs the Func<> delegate and registers into the container
                var factoryDelegate = Expression.Lambda(funcType, registration.BuildExpression()).Compile();
                e.Register(Expression.Constant(factoryDelegate));
            };
        }

        /// <summary>
        ///     Registers the specified delegate that allows returning transient instances of <see cref="TService" />.
        /// </summary>
        /// <typeparam name="TService">Type to be registered.</typeparam>
        /// <param name="instanceCreator"><see cref="Func{T}" /> delegate.</param>
        public void RegisterTransient<TService>(Func<TService> instanceCreator)
            where TService : class
        {
            _container.Register(instanceCreator);
        }
    }
}