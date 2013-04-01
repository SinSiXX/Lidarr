﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marr.Data;
using Marr.Data.Mapping;

namespace NzbDrone.Core.Datastore
{
    public static class MappingExtensions
    {
        public static ColumnMapBuilder<T> RegisterModel<T>(this FluentMappings.MappingsFluentEntity<T> mapBuilder, string tableName) where T : ModelBase
        {
            return mapBuilder.Table.MapTable(tableName)
                             .Columns
                             .AutoMapPropertiesWhere(IsMappableProperty)
                             .For(c => c.Id)
                             .SetPrimaryKey()
                             .SetReturnValue()
                             .SetAutoIncrement();
        }

        public static bool IsMappableProperty(MemberInfo memberInfo)
        {
            var propertyInfo = memberInfo as PropertyInfo;

            if (propertyInfo == null) return false;


            if (!propertyInfo.IsReadable() || !propertyInfo.IsWritable())
            {
                return false;
            }

            if (IsSimpleType(propertyInfo.PropertyType) || MapRepository.Instance.TypeConverters.ContainsKey(propertyInfo.PropertyType))
            {
                return true;
            }

            return false;
        }

        public static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(DateTime)
                   || type == typeof(Decimal);
        }

        private static bool IsReadable(this PropertyInfo propertyInfo)
        {
            return propertyInfo.CanRead && propertyInfo.GetGetMethod(false) != null;
        }

        private static bool IsWritable(this PropertyInfo propertyInfo)
        {
            return propertyInfo.CanWrite && propertyInfo.GetSetMethod(false) != null;
        }
    }
}