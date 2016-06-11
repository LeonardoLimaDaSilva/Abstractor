using System;
using System.Collections.Generic;
using System.Linq;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Identifica se o tipo é uma coleção genérica.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollection(this Type type)
        {
            return type.IsGenericType &&
                   typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                   type.GetInterfaces()
                       .Any(i =>
                           i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(ICollection<>)) || type.IsClass;
        }
    }
}