using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using Abstractor.Cqrs.SimpleInjector.Extensions;

namespace Abstractor.Cqrs.SimpleInjector.EntityFramework.ModelCreation
{
    internal sealed class DefaultDbModelCreator : ICreateDbModel
    {
        private readonly Assembly[] _assemblies;

        public DefaultDbModelCreator(Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            _assemblies = assemblies;
        }

        public void Create(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Encontra todas as configurações de mapeamento definidas no assembly da camada de persistência
            // e cria suas respectivas instâncias via Reflection

            var configurationInstances = _assemblies.Select(
                assembly => assembly
                    .GetSafeTypes()
                    .Where(t => !t.IsAbstract && typeof (StructuralTypeConfiguration<>).IsGenericallyAssignableFrom(t))
                    .ToArray())
                .SelectMany(typesToRegister => typesToRegister.Select(Activator.CreateInstance));

            foreach (var configurationInstance in configurationInstances)
                modelBuilder.Configurations.Add((dynamic) configurationInstance);
        }
    }
}
