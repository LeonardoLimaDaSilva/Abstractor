using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abstractor.Cqrs.Interfaces.CompositionRoot
{
    /// <summary>
    ///     Abstrai o container de inversão de controle.
    /// </summary>
    public interface IContainer
    {
        object GetInstance(Type type);
        object GetCurrentLifetimeScope();
        IDisposable BeginLifetimeScope();
        IEnumerable<IInstanceProducer> GetCurrentRegistrations();
        void RegisterScoped<TService, TImplementation>();
        void RegisterScoped(Type serviceType, Type implementationType);
        void RegisterTransient(Type serviceType, Type implementationType);
        void RegisterTransient(Type openGenericServiceType, IEnumerable<Assembly> assemblies);
        void RegisterSingleton<TService, TImplementation>();
        void RegisterDecoratorTransient(Type serviceType, Type decoratorType);
        void RegisterDecoratorSingleton(Type serviceType, Type decoratorType, Type customAttribute);
        void RegisterDecoratorSingleton(Type serviceType, Type decoratorType);

        void RegisterLazySingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        ///     Permite que delegates do tipo <see cref="Func{T}" /> sejam resolvidos pelo container.
        /// </summary>
        void AllowResolvingFuncFactories();

        void Register<TService>(Func<TService> instanceCreator) where TService : class;
    }
}