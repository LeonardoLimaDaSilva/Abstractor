using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Extensions
{
    /// <summary>
    /// Extensões das entidades.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Obtém o nome da tabela através do atributo <see cref="TableAttribute"/> decorado na classe da entidade.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            var attributes = (TableAttribute[])type.GetCustomAttributes(typeof(TableAttribute), false);
            return attributes.Length > 0
                ? attributes.First().Name
                : type.Name;
        }

        /// <summary>
        /// Transforma um DynamicTableEntity em TEntity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dynamicTableEntity"></param>
        /// <returns></returns>
        public static TEntity Map<TEntity>(this DynamicTableEntity dynamicTableEntity) where TEntity : class, new()
        {
            var entity = new TEntity();
            var entityType = entity.GetType();
            var dictionary = dynamicTableEntity.Properties;

            foreach (var property in entityType.GetProperties())
                foreach (var value in dictionary.Where(value => property.Name == value.Key))
                    property.SetValue(entity,
                        property.PropertyType.IsCollection()
                            ? JsonConvert.DeserializeObject(value.Value.StringValue, property.PropertyType)
                            : GetValue(value.Value));

            return entity;
        }

        private static object GetValue(EntityProperty source)
        {
            switch (source.PropertyType)
            {
                case EdmType.Binary:
                    return source.BinaryValue;
                case EdmType.Boolean:
                    return source.BooleanValue;
                case EdmType.DateTime:
                    return source.DateTime;
                case EdmType.Double:
                    return source.DoubleValue;
                case EdmType.Guid:
                    return source.GuidValue;
                case EdmType.Int32:
                    return source.Int32Value;
                case EdmType.Int64:
                    return source.Int64Value;
                case EdmType.String:
                    return source.StringValue;
                default:
                    throw new TypeLoadException($"EdmType não suportado:{source.PropertyType}");
            }
        }
    }
}