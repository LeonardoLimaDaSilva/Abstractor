using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.Adapters
{
    public sealed class ContainerAdapter : IContainer
    {
        private readonly Container _container;

        public ContainerAdapter(Container container)
        {
            _container = container;
            _container.RegisterSingleton(typeof (IContainer), this);
        }

        public object GetInstance(Type type)
        {
            return _container.GetInstance(type);
        }

        public IEnumerable<object> GetAllInstances(Type type)
        {
            return _container.GetAllInstances(type);
        }

        public object GetCurrentLifetimeScope()
        {
            return _container.GetCurrentLifetimeScope();
        }

        public IDisposable BeginLifetimeScope()
        {
            return _container.BeginLifetimeScope();
        }

        public IEnumerable<IInstanceProducer> GetCurrentRegistrations()
        {
            return _container
                .GetCurrentRegistrations()
                .Select(i => new InstanceProducerAdapter(
                    i.GetInstance(),
                    i.ServiceType));
        }

        public void RegisterScoped<TService, TImplementation>()
        {
            _container.Register(typeof (TService), typeof (TImplementation), Lifestyle.Scoped);
        }

        public void RegisterScoped(Type serviceType, Type implementationType)
        {
            _container.Register(serviceType, implementationType, Lifestyle.Scoped);
        }

        public void RegisterTransient(Type serviceType, Type implementationType)
        {
            _container.Register(serviceType, implementationType, Lifestyle.Transient);
        }

        public void RegisterTransient(Type openGenericServiceType, IEnumerable<Assembly> assemblies)
        {
            _container.Register(openGenericServiceType, assemblies, Lifestyle.Transient);
        }

        public void RegisterCollection(Type openGenericServiceType, IEnumerable<Assembly> assemblies)
        {
            _container.RegisterCollection(openGenericServiceType, assemblies);
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register(typeof (TService), typeof (TImplementation), Lifestyle.Singleton);
        }

        public void RegisterDecoratorTransient(Type serviceType, Type decoratorType)
        {
            _container.RegisterDecorator(serviceType, decoratorType, Lifestyle.Transient);
        }

        public void RegisterDecoratorTransient(Type serviceType, Type decoratorType, Type customAttribute)
        {
            _container.RegisterDecorator(
                serviceType,
                decoratorType,
                Lifestyle.Transient,
                c => c.ImplementationType
                      .CustomAttributes
                      .Any(a => a.AttributeType == customAttribute));
        }

        public void RegisterDecoratorSingleton(Type serviceType, Type decoratorType)
        {
            _container.RegisterDecorator(serviceType, decoratorType, Lifestyle.Singleton);
        }

        public void RegisterLazySingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            var registration = new Lazy<Registration>(() =>
                Lifestyle.Singleton
                         .CreateRegistration<TService, TImplementation>(_container));

            _container.ResolveUnregisteredType += (sender, e) =>
            {
                if (e.UnregisteredServiceType == typeof (TService))
                    e.Register(registration.Value);
            };
        }

        public void AllowResolvingFuncFactories()
        {
            _container.ResolveUnregisteredType += (s, e) =>
            {
                var type = e.UnregisteredServiceType;

                if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof (Func<>)) return;

                var serviceType = type.GetGenericArguments().First();
                var registration = _container.GetRegistration(serviceType, true);
                var funcType = typeof (Func<>).MakeGenericType(serviceType);

                // Constructs the Func<> delegate and registers into the container
                var factoryDelegate = Expression.Lambda(funcType, registration.BuildExpression()).Compile();
                e.Register(Expression.Constant(factoryDelegate));
            };
        }

        public void Register<TService>(Func<TService> instanceCreator) where TService : class
        {
            _container.Register(instanceCreator);
        }
    }
}