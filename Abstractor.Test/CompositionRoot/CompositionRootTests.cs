﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Abstractor.Cqrs.AzureStorage.Blob;
using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.EntityFramework.Extensions;
using Abstractor.Cqrs.EntityFramework.Interfaces;
using Abstractor.Cqrs.Infrastructure.CompositionRoot;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Extensions;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;
using Abstractor.Cqrs.SimpleInjector.Adapters;
using Abstractor.Cqrs.UnitOfWork.Extensions;
using SharpTestsEx;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace Abstractor.Test.CompositionRoot
{
    public class ContextsRegisteringTests
    {
        public class FakeEfContext : IEntityFrameworkContext
        {
            public int SaveChanges()
            {
                return 0;
            }
        }

        [ExcludeFromCodeCoverage]
        public class FakeBlobContext : AzureBlobContext
        {
            public FakeBlobContext() : base("fakeConnectionString")
            {
            }
        }

        [ExcludeFromCodeCoverage]
        public class FakeQueueContext : IAzureQueueContext
        {
            public void Clear()
            {
            }

            public void Rollback()
            {
            }

            public void SaveChanges()
            {
            }
        }

        [ExcludeFromCodeCoverage]
        public class FakeTableContext : IAzureTableContext
        {
            public void Clear()
            {
            }

            public void Rollback()
            {
            }

            public void SaveChanges()
            {
            }
        }

        public class FakeBlob1 : AzureBlob
        {
        }

        public class FakeBlob2 : AzureBlob
        {
        }

        public interface IFakeBlob1Repository
        {
        }

        public interface IFakeBlob2Repository
        {
        }

        public interface IFakeQueue1Repository
        {
        }

        public interface IFakeQueue2Repository
        {
        }

        public class FakeBlob1Repository : BaseBlobRepository<FakeBlob1>, IFakeBlob1Repository
        {
            public FakeBlob1Repository(IAzureBlobRepository<FakeBlob1> repository)
                : base(repository)
            {
            }
        }

        public class FakeBlob2Repository : BaseBlobRepository<FakeBlob1>, IFakeBlob2Repository
        {
            public FakeBlob2Repository(IAzureBlobRepository<FakeBlob1> repository)
                : base(repository)
            {
            }
        }

        public class FakeEfContextWithDependencyInjection : IEntityFrameworkContext
        {
            private readonly ILogger _logger;

            public FakeEfContextWithDependencyInjection(ILogger logger)
            {
                _logger = logger;
            }

            public int SaveChanges()
            {
                _logger.Log("Changes saved.");
                return 0;
            }
        }

        public class FakeLogger : ILogger
        {
            public string Message { get; private set; }

            public void Log(string message)
            {
                Message = message;
            }
        }

        public ContainerAdapter BuildNewAdapter(Container container)
        {
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            container.Options.AllowOverridingRegistrations = true;

            var currentAssembly = new[] {typeof(Helpers.CompositionRoot).Assembly};

            var concreteTypes = currentAssembly
                .GetImplementations(
                    ImplementationConvention.NameEndsWith,
                    new[] {"Repository"}
                ).ToList();

            var containerAdapter = new ContainerAdapter(container);

            containerAdapter.RegisterAbstractor(
                settings =>
                {
                    settings.ApplicationAssemblies = currentAssembly;
                    settings.ApplicationTypes = concreteTypes;
                });

            container.Register<ILogger, FakeLogger>(Lifestyle.Singleton);

            return containerAdapter;
        }

        [Fact]
        public void MultipleBlobRepositories_ShouldResolveWithoutConflicts()
        {
            using (var container = new Container())
            {
                var adapter = BuildNewAdapter(container);

                adapter.RegisterEntityFramework<FakeEfContext>();
                adapter.RegisterAzureBlob<FakeBlobContext>();
                adapter.RegisterAzureQueue<FakeQueueContext>();
                adapter.RegisterAzureTable<FakeTableContext>();
                adapter.RegisterUnitOfWork();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    container.GetInstance<IFakeBlob1Repository>();
                    container.GetInstance<IFakeBlob2Repository>();
                }
            }
        }

        [Fact]
        public void RegisterAzureAlone_GetIUnitOfWorkInstance_ThrowsActivationException()
        {
            using (var container = new Container())
            {
                var adapter = BuildNewAdapter(container);

                adapter.RegisterAzureBlob<FakeBlobContext>();
                adapter.RegisterAzureQueue<FakeQueueContext>();
                adapter.RegisterAzureTable<FakeTableContext>();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    // ReSharper disable once AccessToDisposedClosure
                    Assert.Throws<ActivationException>(() => container.GetInstance<IUnitOfWork>());
                }
            }
        }

        [Fact]
        public void RegisterEntityFrameworkAlone_IUnitOfWorkShouldBeEntityFrameworkUnitOfWork()
        {
            using (var container = new Container())
            {
                var adapter = BuildNewAdapter(container);

                adapter.RegisterEntityFramework<FakeEfContext>();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    var uow = container.GetInstance<IUnitOfWork>();
                    uow.GetType()
                       .FullName.Should()
                       .Be("Abstractor.Cqrs.EntityFramework.Persistence.EntityFrameworkUnitOfWork");

                    uow.Commit();
                    uow.Clear();
                }
            }
        }

        [Fact]
        public void RegisterEntityFrameworkAzureAndUnitOfWork_IUnitOfWorkShouldBeUnitOfWork()
        {
            using (var container = new Container())
            {
                var adapter = BuildNewAdapter(container);

                adapter.RegisterEntityFramework<FakeEfContext>();
                adapter.RegisterAzureBlob<FakeBlobContext>();
                adapter.RegisterAzureQueue<FakeQueueContext>();
                adapter.RegisterAzureTable<FakeTableContext>();
                adapter.RegisterUnitOfWork();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    var uow = container.GetInstance<IUnitOfWork>();
                    uow.GetType().FullName.Should().Be("Abstractor.Cqrs.UnitOfWork.Persistence.UnitOfWork");

                    uow.Commit();
                    uow.Clear();
                }
            }
        }

        [Fact]
        public void RegisterEntityFrameworkWithDependencyInjection_ShouldResolveWithoutErrors()
        {
            using (var container = new Container())
            {
                var adapter = BuildNewAdapter(container);

                adapter.RegisterEntityFramework<FakeEfContextWithDependencyInjection>();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    var logger = (FakeLogger) container.GetInstance<ILogger>();
                    var uow = container.GetInstance<IUnitOfWork>();
                    uow.GetType()
                       .FullName.Should()
                       .Be("Abstractor.Cqrs.EntityFramework.Persistence.EntityFrameworkUnitOfWork");

                    uow.Commit();
                    uow.Clear();

                    logger.Message.Should().Be("Changes saved.");
                }
            }
        }

        [Fact]
        public void RegisterUnitOfWorkAlone_GetIUnitOfWorkInstance_ThrowsActivationException()
        {
            using (var container = new Container())
            {
                var adapter = BuildNewAdapter(container);

                adapter.RegisterUnitOfWork();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    // ReSharper disable once AccessToDisposedClosure
                    Assert.Throws<ActivationException>(() => container.GetInstance<IUnitOfWork>());
                }
            }
        }

        [Fact]
        public void RegisterUnitOfWorkAndEntityFramework_ThrowsActivationException()
        {
            using (var container = new Container())
            {
                var adapter = BuildNewAdapter(container);

                adapter.RegisterUnitOfWork();
                adapter.RegisterEntityFramework<FakeEfContext>();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    // ReSharper disable once AccessToDisposedClosure
                    Assert.Throws<ActivationException>(() => container.GetInstance<IUnitOfWork>());
                }
            }
        }

        [Fact]
        public void RegisterUnitOfWorkAzureAndEntityFramework_IUnitOfWorkShouldBeUnitOfWork()
        {
            using (var container = new Container())
            {
                var adapter = BuildNewAdapter(container);

                adapter.RegisterUnitOfWork();
                adapter.RegisterAzureBlob<FakeBlobContext>();
                adapter.RegisterAzureQueue<FakeQueueContext>();
                adapter.RegisterAzureTable<FakeTableContext>();
                adapter.RegisterEntityFramework<FakeEfContext>();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    var uow = container.GetInstance<IUnitOfWork>();
                    uow.GetType().FullName.Should().Be("Abstractor.Cqrs.UnitOfWork.Persistence.UnitOfWork");

                    uow.Commit();
                    uow.Clear();
                }
            }
        }
    }
}