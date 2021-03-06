﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abstractor.Cqrs.Interfaces.CompositionRoot
{
    /// <summary>
    ///     Abstraction of the inversion of control container that exposes only the methods required by the framework.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        ///     Provides functionality for resolving delegates of type <see cref="Func{TResult}" />.
        /// </summary>
        void AllowResolvingFuncFactories();

        /// <summary>
        ///     Begins a new scope for the given container.
        /// </summary>
        /// <returns>New scope.</returns>
        IDisposable BeginLifetimeScope();

        /// <summary>
        ///     Gets all instances registered for the given type.
        /// </summary>
        /// <param name="type">Type of instances.</param>
        /// <returns>Instances registered for given type.</returns>
        IEnumerable<object> GetAllInstances(Type type);

        /// <summary>
        ///     Gets the current scope.
        /// </summary>
        /// <returns>Current scope.</returns>
        object GetCurrentLifetimeScope();

        /// <summary>
        ///     Gets and instance of the given type.
        /// </summary>
        /// <param name="type">Type of instance.</param>
        /// <returns>Instance of given type.</returns>
        object GetInstance(Type type);

        /// <summary>
        ///     Verify whether the exception is an activation exception.
        /// </summary>
        /// <param name="exception">Exception to be verified.</param>
        /// <returns></returns>
        bool IsActivationException(Exception exception);

        /// <summary>
        ///     Registers all concrete types that implements an open generic abstraction.
        /// </summary>
        /// <param name="openGenericServiceType">Type of the abstraction.</param>
        /// <param name="assemblies">Assemblies that contains the concrete types.</param>
        void RegisterCollection(Type openGenericServiceType, IEnumerable<Assembly> assemblies);

        /// <summary>
        ///     Ensures that the same instance of the supplied decoratorType is returned, wrapping the original
        ///     serviceType.
        /// </summary>
        /// <param name="serviceType">Original type.</param>
        /// <param name="decoratorType">Type that wraps the original type.</param>
        void RegisterDecoratorScoped(Type serviceType, Type decoratorType);

        /// <summary>
        ///     Ensures that the same instance of the supplied decoratorType is returned, wrapping the original
        ///     serviceType.
        /// </summary>
        /// <param name="serviceType">Original type.</param>
        /// <param name="decoratorType">Type that wraps the original type.</param>
        void RegisterDecoratorSingleton(Type serviceType, Type decoratorType);

        /// <summary>
        ///     Ensures that the a new instance of the supplied decoratorType is returned, wrapping the original
        ///     serviceType.
        /// </summary>
        /// <param name="serviceType">Original type.</param>
        /// <param name="decoratorType">Type that wraps the original type.</param>
        void RegisterDecoratorTransient(Type serviceType, Type decoratorType);

        /// <summary>
        ///     Provides functionality for the deferred resolving of unregistered types.
        /// </summary>
        /// <typeparam name="TService">Type of abstraction.</typeparam>
        /// <typeparam name="TImplementation">Type of implementation.</typeparam>
        void RegisterLazyScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        ///     Provides functionality for the deferred resolving of unregistered types.
        /// </summary>
        /// <typeparam name="TService">Type of abstraction.</typeparam>
        /// <typeparam name="TImplementation">Type of implementation.</typeparam>
        void RegisterLazySingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        ///     Registers that an instance of type TImplementation will be returned when an instance of type
        ///     TService is requested. Uses the container's configured scope lifetime.
        /// </summary>
        /// <typeparam name="TService">Type of abstraction.</typeparam>
        /// <typeparam name="TImplementation">Type of implementation.</typeparam>
        void RegisterScoped<TService, TImplementation>();

        /// <summary>
        ///     Registers that an instance of type serviceType will be returned when an instance of type
        ///     implementationType is requested. Uses the container's configured scope lifetime.
        /// </summary>
        /// <param name="serviceType">Type of abstraction.</param>
        /// <param name="implementationType">Type of implementation.</param>
        void RegisterScoped(Type serviceType, Type implementationType);

        /// <summary>
        ///     Registers that an instance of type TImplementation will return the same instance of type
        ///     TService when requested.
        /// </summary>
        /// <typeparam name="TService">Type of abstraction.</typeparam>
        /// <typeparam name="TImplementation">Type of the implementation.</typeparam>
        void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        ///     Registers the specified delegate that allows returning singleton instances of TService.
        /// </summary>
        /// <typeparam name="TService">Type to be registered.</typeparam>
        /// <param name="instanceCreator"><see cref="Func{T}" /> delegate.</param>
        void RegisterSingleton<TService>(Func<TService> instanceCreator)
            where TService : class;

        /// <summary>
        ///     Registers that a new instance of type serviceType will be returned each time that an instance of
        ///     type implementationType is requested.
        /// </summary>
        /// <param name="serviceType">Type of the abstraction.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        void RegisterTransient(Type serviceType, Type implementationType);

        /// <summary>
        ///     Registers all concrete types contained in the assemblies that implements an open generic abstraction, and returns a
        ///     new instance each time an instance of openGenericServiceType is requested.
        /// </summary>
        /// <param name="openGenericServiceType">Type of the abstraction.</param>
        /// <param name="assemblies">Assemblies that contains the concrete type</param>
        void RegisterTransient(Type openGenericServiceType, IEnumerable<Assembly> assemblies);
    }
}