using System;
using System.Collections.Generic;
using System.Data;

namespace Wooly905.FlowTx.Impl;

internal static class FlowTxExtensions
{
    /// <summary>
    /// Get SQL command timeout in seconds
    /// </summary>
    public static int CommandTimeout => 60;

    public static T GetValueOrDefault<T>(this IDataReader dataReader, string fieldName)
    {
        if (!dataReader.ColumnExists(fieldName))
        {
            return default;
        }

        if (dataReader[fieldName] == DBNull.Value)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)string.Empty;
            }

            return default;
        }

        return (T)dataReader[fieldName];
    }

    public static bool TryGetValue<T>(this IDataReader dataReader, string fieldName, out T value)
    {
        if (!dataReader.ColumnExists(fieldName))
        {
            value = default;
            return false;
        }

        if (dataReader[fieldName] != DBNull.Value)
        {
            value = (T)dataReader[fieldName];
            return true;
        }

        if (typeof(T) == typeof(string))
        {
            value = (T)(object)string.Empty;
            return true;
        }

        value = default;
        return true;
    }

    public static bool ColumnExists(this IDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static void PrepareParameters(this IDbCommand command, IEnumerable<IDbDataParameter> parameters)
    {
        if (parameters == null)
        {
            return;
        }

        foreach (IDbDataParameter parameter in parameters)
        {
            // Check for derived output value with no value assigned
            if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input)
                && parameter.Value == null)
            {
                parameter.Value = DBNull.Value;
            }

            command.Parameters.Add(parameter);
        }
    }
}
