using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Wooly905.FlowTx.Impl;

internal static class ModelValuesMapper
{
    private static readonly char[] _delimiters = { '.' };

    public static T MapTo<T>(IEnumerable<(string, string, Type)> mappings,
                             IDataReader dataReader,
                             IReadOnlyDictionary<string, Func<object, object>> converters = null)
    {
        Type destinationType = typeof(T);
        T instance = (T)Activator.CreateInstance(destinationType, true);
        PropertyInfo[] properties = destinationType.GetProperties();

        foreach ((string ModelPropertyPath, string ColumnName, Type ModelPropertyType) mapping in mappings)
        {
            FindAndSetPropertyValue(instance,
                                    properties,
                                    dataReader,
                                    mapping,
                                    GetConvertFunc(converters, mapping.ModelPropertyPath));
        }

        return instance;
    }

    private static void FindAndSetPropertyValue<T>(T instance,
                                                   PropertyInfo[] properties,
                                                   IDataReader dataReader,
                                                   (string ModelPropertyPath, string ColumnName, Type ModelPropertyType) mapping,
                                                   Func<object, object> converter)
    {
        (string FirstPropertyName, string SubPath) = SplitPropertyPath(mapping.ModelPropertyPath);

        if (string.IsNullOrEmpty(FirstPropertyName))
        {
            return;
        }

        PropertyInfo property = Array.Find(properties, x => string.Equals(x.Name, FirstPropertyName, StringComparison.OrdinalIgnoreCase));

        if (property == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(SubPath))
        {
            SetPropertyValue(dataReader, mapping, converter, instance, property);
        }
        else
        {
            Type propertyType = property.PropertyType;

            if (!propertyType.IsClass)
            {
                return;
            }

            object propertyInstance = GetOrCreatePropertyInstance(instance, property, propertyType);

            FindAndSetPropertyValue(propertyInstance,
                                    propertyType.GetProperties(),
                                    dataReader,
                                    (SubPath, mapping.ColumnName, mapping.ModelPropertyType),
                                    converter);
        }
    }

    private static Func<object, object> GetConvertFunc(IReadOnlyDictionary<string, Func<object, object>> converters,
                                                       string modelPropertyPath)
    {
        if (converters is not null
            && converters.TryGetValue(modelPropertyPath, out Func<object, object> convertFunc))
        {
            return convertFunc;
        }

        return x => x;
    }

    private static (string, string) SplitPropertyPath(string modelPropertyPath)
    {
        string[] paths = modelPropertyPath.Split(_delimiters, 2, StringSplitOptions.RemoveEmptyEntries);

        return (paths.Length >= 1 ? paths[0] : string.Empty, paths.Length >= 2 ? paths[1] : string.Empty);
    }

    private static void SetPropertyValue<T>(IDataReader dataReader,
                                            (string ModelPropertyPath, string ColumnName, Type ModelPropertyType) mapping,
                                            Func<object, object> converter,
                                            T instance,
                                            PropertyInfo property)
    {
        object? value = GetDataReaderValue(dataReader,
                                           mapping.ColumnName,
                                           mapping.ModelPropertyType,
                                           converter);

        if (value is not null)
        {
            property.SetValue(instance, value);
        }
    }

    private static object? GetDataReaderValue(IDataReader dataReader,
                                              string columnName,
                                              Type datayType,
                                              Func<object, object> converter)
    {
        object? value = GetDataReaderValue(dataReader, datayType, columnName);

        if (converter != null)
        {
            value = converter(value);
        }

        return value;
    }

    private static object? GetDataReaderValue(IDataReader reader, Type propertyType, string columnName)
    {
        if (propertyType == typeof(string) && reader.ColumnExists(columnName))
        {
            return reader[columnName] == DBNull.Value
                   ? string.Empty
                   : reader[columnName].ToString();
        }

        if (propertyType == typeof(int) && TryGetIntegerValue(reader, columnName, out int intValue))
        {
            return intValue;
        }

        if (propertyType == typeof(int?) && TryGetNullableIntegerValue(reader, columnName, out int? intValue2))
        {
            return intValue2;
        }

        if (propertyType == typeof(bool) && TryGetBooleanValue(reader, columnName, out bool boolValue))
        {
            return boolValue;
        }

        if (propertyType == typeof(bool?) && TryGetNullableBooleanValue(reader, columnName, out bool? boolValue2))
        {
            return boolValue2;
        }

        if (propertyType == typeof(DateTime) && TryGetDateTimeValue(reader, columnName, out DateTime dateValue))
        {
            return dateValue;
        }

        if (propertyType == typeof(DateTime?) && TryGetNullableDateTimeValue(reader, columnName, out DateTime? dateValue2))
        {
            return dateValue2;
        }

        if (propertyType == typeof(byte) && TryGetByteValue(reader, columnName, out byte byteValue))
        {
            return byteValue;
        }

        if (propertyType == typeof(byte?) && TryGetNullableByteValue(reader, columnName, out byte? byteValue2))
        {
            return byteValue2;
        }

        if (propertyType == typeof(short) && TryGetShortValue(reader, columnName, out short shortValue))
        {
            return shortValue;
        }

        if (propertyType == typeof(short?) && TryGetNullableShortValue(reader, columnName, out short? shortValue2))
        {
            return shortValue2;
        }

        if (propertyType == typeof(long) && TryGetLongValue(reader, columnName, out long longValue))
        {
            return longValue;
        }

        if (propertyType == typeof(long?) && TryGetNullableLongValue(reader, columnName, out long? long2Value))
        {
            return long2Value;
        }

        if (propertyType == typeof(decimal) && TryGetDecimalValue(reader, columnName, out decimal decimalValue))
        {
            return decimalValue;
        }

        if (propertyType == typeof(decimal?) && TryGetNullableDecimalValue(reader, columnName, out decimal? decimalValue2))
        {
            return decimalValue2;
        }

        if (propertyType == typeof(double) && TryGetDoubleValue(reader, columnName, out double doubleValue))
        {
            return doubleValue;
        }

        if (propertyType == typeof(double?) && TryGetNullableDoubleValue(reader, columnName, out double? doubleValue2))
        {
            return doubleValue2;
        }

        if (propertyType == typeof(Guid) && TryGetGuidValue(reader, columnName, out Guid guidValue))
        {
            return guidValue;
        }

        return null;
    }

    private static bool TryGetGuidValue(IDataReader reader, string columnName, out Guid value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && Guid.TryParse(reader[columnName].ToString(), out Guid value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetDoubleValue(IDataReader reader, string columnName, out double value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && double.TryParse(reader[columnName].ToString(), out double value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetNullableDoubleValue(IDataReader reader, string columnName, out double? value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = null;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && double.TryParse(reader[columnName].ToString(), out double value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return true;
    }

    private static bool TryGetDecimalValue(IDataReader reader, string columnName, out decimal value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && decimal.TryParse(reader[columnName].ToString(), out decimal value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetNullableDecimalValue(IDataReader reader, string columnName, out decimal? value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = null;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && decimal.TryParse(reader[columnName].ToString(), out decimal value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return true;
    }

    private static bool TryGetLongValue(IDataReader reader, string columnName, out long value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && long.TryParse(reader[columnName].ToString(), out long value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetNullableLongValue(IDataReader reader, string columnName, out long? value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = null;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && long.TryParse(reader[columnName].ToString(), out long value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return true;
    }

    private static bool TryGetShortValue(IDataReader reader, string columnName, out short value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && short.TryParse(reader[columnName].ToString(), out short value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetNullableShortValue(IDataReader reader, string columnName, out short? value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = null;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && short.TryParse(reader[columnName].ToString(), out short value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return true;
    }

    private static bool TryGetDateTimeValue(IDataReader reader, string columnName, out DateTime value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && DateTime.TryParse(reader[columnName].ToString(), out DateTime value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetNullableDateTimeValue(IDataReader reader, string columnName, out DateTime? value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = null;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && DateTime.TryParse(reader[columnName].ToString(), out DateTime value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return true;
    }

    private static bool TryGetBooleanValue(IDataReader reader, string columnName, out bool value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && bool.TryParse(reader[columnName].ToString(), out bool value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetNullableBooleanValue(IDataReader reader, string columnName, out bool? value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = null;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && bool.TryParse(reader[columnName].ToString(), out bool value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return true;
    }

    private static bool TryGetIntegerValue(IDataReader reader, string columnName, out int value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && int.TryParse(reader[columnName].ToString(), out int value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetNullableIntegerValue(IDataReader reader, string columnName, out int? value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = null;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && int.TryParse(reader[columnName].ToString(), out int value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return true;
    }

    private static bool TryGetByteValue(IDataReader reader, string columnName, out byte value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = default;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && byte.TryParse(reader[columnName].ToString(), out byte value2))
        {
            value = value2;
            return true;
        }

        value = default;
        return true;
    }

    private static bool TryGetNullableByteValue(IDataReader reader, string columnName, out byte? value)
    {
        if (!reader.ColumnExists(columnName))
        {
            value = null;
            return false;
        }

        if (reader[columnName] != DBNull.Value
            && byte.TryParse(reader[columnName].ToString(), out byte value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return true;
    }

    private static object GetOrCreatePropertyInstance<T>(T instance, PropertyInfo property, Type propertyType)
    {
        object propertyInstance = property.GetValue(instance);

        if (propertyInstance == null)
        {
            propertyInstance = Activator.CreateInstance(propertyType, true);
            property.SetValue(instance, propertyInstance);
        }

        return propertyInstance;
    }
}
