using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Extensions
{
    /// <summary>
    ///     Auxiliary extensions for reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        ///     Discovers by convention and returns the implementation types inside the assemblies.
        /// </summary>
        /// <param name="assemblies">Assemblies where the search will be made.</param>
        /// <param name="convention">Convention algorith for the search.</param>
        /// <param name="names">Names to be searched.</param>
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

        /// <summary>
        ///     Discovers the injectable implementation types inside the assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetImplementations(this IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.GetInterfaces().Any())
                .AsQueryable()
                .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(InjectableAttribute)))
                .Select(t => t.AsType())
                .ToList();
        }

        /// <summary>
        ///     Ensures the return of types that can be loaded from an assembly.
        /// </summary>
        /// <param name="assembly">Assembly to be analized.</param>
        /// <returns></returns>
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
    }
}