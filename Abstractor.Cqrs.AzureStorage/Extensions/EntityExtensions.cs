using System;
using System.Linq;
using Abstractor.Cqrs.AzureStorage.Attributes;

namespace Abstractor.Cqrs.AzureStorage.Extensions
{
    /// <summary>
    ///     Extensões das entidades.
    /// </summary>
    internal static class EntityExtensions
    {
        /// <summary>
        ///     Obtém o nome da tabela através do atributo <see cref="AzureTableAttribute" /> decorado na classe da entidade.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            var attributes = (AzureTableAttribute[]) type.GetCustomAttributes(typeof (AzureTableAttribute), false);
            return attributes.Length > 0
                ? attributes.First().Name
                : type.Name;
        }

        /// <summary>
        ///     Obtém o nome do container através do atributo <see cref="AzureContainerAttribute" /> decorado na classe da
        ///     entidade.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetContainerName(this Type type)
        {
            var attributes =
                (AzureContainerAttribute[]) type.GetCustomAttributes(typeof (AzureContainerAttribute), false);
            return attributes.Length > 0
                ? attributes.First().Name.ToLowerInvariant()
                : type.Name.ToLowerInvariant();
        }

        /// <summary>
        ///     Obtém o nome da fila através do atributo <see cref="AzureQueueAttribute" /> decorado na classe da entidade.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetQueueName(this Type type)
        {
            var attributes = (AzureQueueAttribute[]) type.GetCustomAttributes(typeof (AzureQueueAttribute), false);
            return attributes.Length > 0
                ? attributes.First().Name.ToLowerInvariant()
                : type.Name.ToLowerInvariant();
        }
    }
}