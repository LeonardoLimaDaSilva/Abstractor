using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Persistence
{
    public class TableEntityDto : DynamicObject, ITableEntity
    {
        public IDictionary<string, EntityProperty> Properties { get; private set; }

        public bool IsDirty { get; set; } = false;

        public string ETag { get; set; }

        public string PartitionKey { get; set; }
        
        public string RowKey { get; set; }
        
        public DateTimeOffset Timestamp { get; set; }

        public TableEntityDto()
        {
            Properties = new Dictionary<string, EntityProperty>();
        }

        public TableEntityDto(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Properties = new Dictionary<string, EntityProperty>();
        }
        
        private static EntityProperty ConvertToEntityProperty(string key, object value)
        {
            if (value == null) return new EntityProperty((string)null);

            if (value.GetType() == typeof(byte[])) return new EntityProperty((byte[])value);

            if (value is bool) return new EntityProperty((bool)value);

            if (value is DateTimeOffset) return new EntityProperty((DateTimeOffset)value);

            if (value is DateTime) return new EntityProperty((DateTime)value);

            if (value is double) return new EntityProperty((double)value);

            if (value is Guid) return new EntityProperty((Guid)value);

            if (value is int) return new EntityProperty((int)value);

            if (value is long) return new EntityProperty((long)value);

            var stringValue = value as string;
            if (stringValue != null) return new EntityProperty(stringValue);

            throw new Exception($"O tipo {value.GetType()} não é suportado para {key}.");
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!Properties.ContainsKey(binder.Name))
                Properties.Add(binder.Name, ConvertToEntityProperty(binder.Name, null));

            result = Properties[binder.Name];

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var property = ConvertToEntityProperty(binder.Name, value);

            if (Properties.ContainsKey(binder.Name))
                Properties[binder.Name] = property;
            else
                Properties.Add(binder.Name, property);

            return true;
        }

        public bool TrySetMember(string binder, object value)
        {
            var property = ConvertToEntityProperty(binder, value);

            if (Properties.ContainsKey(binder))
                Properties[binder] = property;
            else
                Properties.Add(binder, property);

            return true;
        }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Properties = properties;
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return Properties;
        }
    }
}