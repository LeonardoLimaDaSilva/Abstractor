using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Extensions
{
    /// <summary>
    ///     Extensões auxiliares para reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        ///     Garante o retorno apenas dos tipos que podem ser carregados de um assembly.
        /// </summary>
        /// <param name="assembly">O assembly que será analisado.</param>
        /// <returns>Um array de tipos.</returns>
        internal static Type[] GetSafeTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }

        /// <summary>
        ///     Retorna os tipos de implementação contidos nos assemblies. Os tipos são encontrados através de convenções.
        /// </summary>
        /// <param name="assemblies">Lista de assemblies em que a busca será feita</param>
        /// <param name="convention">Convenção usada para a busca.</param>
        /// <param name="names">Lista de nomes usados na busca.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetImplementations(
            this IEnumerable<Assembly> assemblies,
            ImplementationConvention convention,
            string[] names)
        {
            var predicate = PredicateBuilder.False<TypeInfo>();

            switch (convention)
            {
                case ImplementationConvention.NameStartsWith:
                    predicate = names.Aggregate(predicate, (current, temp) => current.Or(p => p.Name.StartsWith(temp)));
                    break;
                case ImplementationConvention.NameEndsWith:
                    predicate = names.Aggregate(predicate, (current, temp) => current.Or(p => p.Name.EndsWith(temp)));
                    break;
                case ImplementationConvention.NameContains:
                    predicate = names.Aggregate(predicate, (current, temp) => current.Or(p => p.Name.Contains(temp)));
                    break;
                case ImplementationConvention.NamespaceStartsWith:
                    predicate = names.Aggregate(predicate,
                        (current, temp) => current.Or(p => p.Namespace.StartsWith(temp)));
                    break;
                case ImplementationConvention.NamespaceEndsWith:
                    predicate = names.Aggregate(predicate,
                        (current, temp) => current.Or(p => p.Namespace.EndsWith(temp)));
                    break;
                case ImplementationConvention.NamespaceContains:
                    predicate = names.Aggregate(predicate,
                        (current, temp) => current.Or(p => p.Namespace.Contains(temp)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(convention), convention, null);
            }

            return assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.GetInterfaces().Any())
                .AsQueryable()
                .Where(predicate)
                .Select(t => t.AsType())
                .ToList();
        }
    }
}