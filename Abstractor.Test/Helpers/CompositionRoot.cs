using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.CompositionRoot;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Extensions;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;
using Abstractor.Cqrs.SimpleInjector.Adapters;
using SimpleInjector;
using SimpleInjector.Extensions.LifetimeScoping;

namespace Abstractor.Test.Helpers
{
    /// <summary>
    ///     Composition root implemented as a thread-safe singleton.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CompositionRoot
    {
        private static readonly object Lock = new object();
        private static volatile Container _container;

        private CompositionRoot()
        {
        }

        public static Container GetContainer()
        {
            // Double-checked locking pattern

            // ReSharper disable once InvertIf
            if (_container == null)
            {
                lock (Lock)
                {
                    if (_container != null) return _container;

                    _container = new Container();

                    _container.Options.DefaultScopedLifestyle = new LifetimeScopeLifestyle();

                    var currentAssembly = new[] {typeof (CompositionRoot).Assembly};

                    var concreteTypes = currentAssembly
                        .GetImplementations(
                            ImplementationConvention.NameEndsWith,
                            new[] {"Repository"}
                        ).ToList();

                    var containerAdapter = new ContainerAdapter(_container);

                    containerAdapter.RegisterAbstractor(
                        cs =>
                        {
                            cs.ApplicationAssemblies = currentAssembly;
                            cs.ApplicationTypes = concreteTypes;
                        });

                    containerAdapter.RegisterSingleton<IUnitOfWork, FakeUnitOfWork>();
                    containerAdapter.RegisterSingleton<IStopwatch, FakeStopwatch>();
                    containerAdapter.RegisterScoped<ILogger, FakeLogger>();

                    _container.Verify();
                }
            }

            return _container;
        }
    }
}