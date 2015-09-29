using System;
using System.Linq;
using System.Reflection;

namespace Abstractor.Cqrs.SimpleInjector.Extensions
{
    /// <summary>
    /// Extensões auxiliares para reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Verifica se um tipo é genericamente associável ao outro tipo.
        /// </summary>
        /// <param name="openGeneric">Tipo aberto.</param>
        /// <param name="closedGeneric">Tipo fechado.</param>
        /// <returns>Verdadeiro se os tipos são associáveis.</returns>
        public static bool IsGenericallyAssignableFrom(this Type openGeneric, Type closedGeneric)
        {
            var interfaceTypes = closedGeneric.GetInterfaces();

            if (interfaceTypes.Where(interfaceType => interfaceType.IsGenericType).Any(interfaceType => interfaceType.GetGenericTypeDefinition() == openGeneric))
                return true;

            var baseType = closedGeneric.BaseType;
            if (baseType == null) return false;

            return baseType.IsGenericType &&
                (baseType.GetGenericTypeDefinition() == openGeneric ||
                openGeneric.IsGenericallyAssignableFrom(baseType));
        }

        /// <summary>
        /// Garante o retorno apenas dos tipos que podem ser carregados de um assembly.
        /// </summary>
        /// <param name="assembly">O assembly que será analisado.</param>
        /// <returns>Um array de tipos.</returns>
        public static Type[] GetSafeTypes(this Assembly assembly)
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
