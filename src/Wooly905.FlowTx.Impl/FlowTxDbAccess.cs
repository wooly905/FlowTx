using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Wooly905.FlowTx.Abstraction;

namespace Wooly905.FlowTx.Impl;

public class FlowTxDbAccess(string connectionString) : IFlowTxDbAccess
{
    public async Task<int> ExecuteNonQueryAsync(string storedProcedureName, int? commandTimeout = null)
    {
        return await ExecuteNonQueryInternalAsync(storedProcedureName, null, commandTimeout).ConfigureAwait(false);
    }

    public async Task<int> ExecuteNonQueryAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters, int? commandTimeout = null)
    {
        return await ExecuteNonQueryInternalAsync(storedProcedureName, parameters, commandTimeout).ConfigureAwait(false);
    }

    private async Task<int> ExecuteNonQueryInternalAsync(string storedProcedureName, IEnumerable<IDbDataParameter>? parameters, int? commandTimeout = null)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        using SqlCommand command = parameters == null
                                   ? CreateSqlCommand(connection, storedProcedureName, CommandType.StoredProcedure, commandTimeout)
                                   : CreateSqlCommand(connection, storedProcedureName, CommandType.StoredProcedure, parameters, commandTimeout);

        return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    internal static SqlCommand CreateSqlCommand(SqlConnection connection,
                                                string commandText,
                                                CommandType commandType,
                                                IEnumerable<IDbDataParameter> parameters,
                                                int? commandTimeout = null)
    {
        SqlCommand command = CreateSqlCommand(connection,
                                              commandText,
                                              commandType,
                                              commandTimeout);

        if (parameters != null)
        {
            command.PrepareParameters(parameters);
        }

        return command;
    }

    internal static SqlCommand CreateSqlCommand(SqlConnection connection,
                                                string commandText,
                                                CommandType commandType,
                                                int? commandTimeout = null)
    {
        return new SqlCommand()
        {
            CommandType = commandType,
            CommandText = commandText,
            Connection = connection,
            CommandTimeout = commandTimeout ?? FlowTxExtensions.CommandTimeout
        };
    }

    public async Task<int> ExecuteNonQueryByCmdTextAsync(string commandText, int? commandTimeout = null)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        using SqlCommand command = CreateSqlCommand(connection, commandText, CommandType.Text, commandTimeout);

        return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task<int> ExecuteNonQueryByCmdTextAsync(string commandText,
                                                         IEnumerable<IDbDataParameter> parameters,
                                                         int? commandTimeout = null)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        using SqlCommand command = CreateSqlCommand(connection, commandText, CommandType.Text, parameters, commandTimeout);

        return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task<object> ExecuteScalarAsync(string storedProcedureName,
                                                 int? commandTimeout = null)
    {
        return await ExecuteScalarInternalAsync(storedProcedureName, null, commandTimeout).ConfigureAwait(false);
    }

    public async Task<object> ExecuteScalarAsync(string storedProcedureName,
                                                 IEnumerable<IDbDataParameter> parameters,
                                                 int? commandTimeout = null)
    {
        return await ExecuteScalarInternalAsync(storedProcedureName, parameters, commandTimeout).ConfigureAwait(false);
    }

    private async Task<object> ExecuteScalarInternalAsync(string storedProcedureName,
                                                          IEnumerable<IDbDataParameter>? parameters,
                                                          int? commandTimeout = null)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        using SqlCommand command = parameters == null
                                   ? CreateSqlCommand(connection, storedProcedureName, CommandType.StoredProcedure, commandTimeout)
                                   : CreateSqlCommand(connection, storedProcedureName, CommandType.StoredProcedure, parameters, commandTimeout);

        return await command.ExecuteScalarAsync().ConfigureAwait(false);
    }

    public async Task<object> ExecuteScalarByCmdTextAsync(string commandText, int? commandTimeout = null)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        using SqlCommand command = CreateSqlCommand(connection,
                                                    commandText,
                                                    CommandType.Text,
                                                    commandTimeout);

        return await command.ExecuteScalarAsync().ConfigureAwait(false);
    }

    public async Task<IFlowTxDbReader> GetDataReaderAsync(string commandText, int? commandTimeout = null)
    {
        return await GetDataReaderInternalAsync(commandText,
                                                CommandType.Text,
                                                null,
                                                commandTimeout).ConfigureAwait(false);
    }

    public async Task<IFlowTxDbReader> GetDataReaderAsync(string storedProcedureName,
                                                          IEnumerable<IDbDataParameter> parameters,
                                                          int? commandTimeout = null)
    {
        return await GetDataReaderInternalAsync(storedProcedureName,
                                                CommandType.StoredProcedure,
                                                parameters,
                                                commandTimeout).ConfigureAwait(false);
    }

    private async Task<IFlowTxDbReader> GetDataReaderInternalAsync(string commandText,
                                                                   CommandType commandType,
                                                                   IEnumerable<IDbDataParameter> parameters,
                                                                   int? commandTimeout = null)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        using SqlCommand command = parameters == null
                                   ? CreateSqlCommand(connection, commandText, commandType, commandTimeout)
                                   : CreateSqlCommand(connection, commandText, commandType, parameters, commandTimeout);

        using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        List<Dictionary<string, object>> columnRecords = new();
        List<string> columnNames = new();

        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            Dictionary<string, object> records = new(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
                records[reader.GetName(i)] = ParseValue(reader, i);
            }

            columnRecords.Add(records);
        }

        return new FlowTxDataReader(columnNames, columnRecords);
    }

    private static object ParseValue(SqlDataReader reader, int recordIndex)
    {
        if (reader.IsDBNull(recordIndex))
        {
            return null;
        }

        if (reader.GetDataTypeName(recordIndex).IndexOf("char", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return reader.GetString(recordIndex);
        }

        if (string.Equals(reader.GetDataTypeName(recordIndex), "tinyint", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(reader.GetByte(recordIndex).ToString(), out int byteValue))
        {
            return byteValue;
        }

        if (string.Equals(reader.GetDataTypeName(recordIndex), "smallint", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(reader.GetInt16(recordIndex).ToString(), out int smallIntValue))
        {
            return smallIntValue;
        }

        if (string.Equals(reader.GetDataTypeName(recordIndex), "int", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(reader.GetInt32(recordIndex).ToString(), out int intValue))
        {
            return intValue;
        }

        if (string.Equals(reader.GetDataTypeName(recordIndex), "bigint", StringComparison.OrdinalIgnoreCase)
            && long.TryParse(reader.GetInt64(recordIndex).ToString(), out long longValue))
        {
            return longValue;
        }

        if (reader.GetDataTypeName(recordIndex).IndexOf("date", StringComparison.OrdinalIgnoreCase) >= 0
            && DateTime.TryParse(reader.GetDateTime(recordIndex).ToString(), out DateTime dateValue))
        {
            return dateValue;
        }

        if (string.Equals(reader.GetDataTypeName(recordIndex), "bit", StringComparison.OrdinalIgnoreCase)
            && bool.TryParse(reader.GetBoolean(recordIndex).ToString(), out bool boolValue))
        {
            return boolValue;
        }

        return null;
    }

    public async Task<T?> GetRecordAsync<T>(string storedProcedureName,
                                            IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                            int? commandTimeout = null) where T : class
    {
        return await GetRecordAsync<T>(storedProcedureName,
                                       null,
                                       mappings,
                                       commandTimeout).ConfigureAwait(false);
    }

    public async Task<T?> GetRecordAsync<T>(string storedProcedureName,
                                            IEnumerable<IDbDataParameter> parameters,
                                            IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                            int? commandTimeout = null) where T : class
    {
        return await GetRecordInternalAsync<T>(storedProcedureName,
                                               CommandType.StoredProcedure,
                                               parameters,
                                               mappings,
                                               null,
                                               commandTimeout).ConfigureAwait(false);
    }

    public async Task<T?> GetRecordAsync<T>(string storedProcedureName,
                                           IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                           IReadOnlyDictionary<string, Func<object, object>> converters,
                                           int? commandTimeout = null) where T : class
    {
        return await GetRecordInternalAsync<T>(storedProcedureName,
                                               CommandType.StoredProcedure,
                                               null,
                                               mappings,
                                               converters,
                                               commandTimeout).ConfigureAwait(false);
    }

    public async Task<T?> GetRecordAsync<T>(string storedProcedureName,
                                            IEnumerable<IDbDataParameter> parameters,
                                            IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                            IReadOnlyDictionary<string, Func<object, object>> converters,
                                            int? commandTimeout = null) where T : class
    {
        return await GetRecordInternalAsync<T>(storedProcedureName,
                                               CommandType.StoredProcedure,
                                               parameters,
                                               mappings,
                                               converters,
                                               commandTimeout).ConfigureAwait(false);
    }

    private async Task<T?> GetRecordInternalAsync<T>(string commandText,
                                                     CommandType commandType,
                                                     IEnumerable<IDbDataParameter> parameters,
                                                     IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                     IReadOnlyDictionary<string, Func<object, object>> converters,
                                                     int? commandTimeout = null) where T : class
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            throw new ArgumentNullException(nameof(commandText), "Command text cannot be null or empty!");
        }

        if (mappings?.Any() != true)
        {
            throw new ArgumentException("Mapping cannot be null or empty");
        }

        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        using SqlCommand command = parameters == null
                                   ? CreateSqlCommand(connection, commandText, commandType, commandTimeout)
                                   : CreateSqlCommand(connection, commandText, commandType, parameters, commandTimeout);
        using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        if (await reader.ReadAsync().ConfigureAwait(false))
        {
            if (converters?.Any() == true)
            {
                return ModelValuesMapper.MapTo<T>(mappings, reader, converters);
            }

            return ModelValuesMapper.MapTo<T>(mappings, reader, null);
        }

        return null;
    }

    public async Task<T?> GetRecordByCmdTextAsync<T>(string commandText,
                                                     IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                     int? commandTimeout = null) where T : class
    {
        return await GetRecordInternalAsync<T>(commandText,
                                               CommandType.Text,
                                               null,
                                               mappings,
                                               null,
                                               commandTimeout).ConfigureAwait(false);
    }

    public async Task<T?> GetRecordByCmdTextAsync<T>(string commandText,
                                                     IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                     IReadOnlyDictionary<string, Func<object, object>> converters,
                                                     int? commandTimeout = null) where T : class
    {
        return await GetRecordInternalAsync<T>(commandText,
                                               CommandType.Text,
                                               null,
                                               mappings,
                                               converters,
                                               commandTimeout).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetRecordsAsync<T>(string storedProcedureName,
                                                         IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                         int? commandTimeout = null) where T : class
    {
        return await GetRecordsInternalAsync<T>(storedProcedureName,
                                                CommandType.StoredProcedure,
                                                null,
                                                mappings,
                                                null,
                                                commandTimeout).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetRecordsAsync<T>(string storedProcedureName,
                                                         IEnumerable<IDbDataParameter> parameters,
                                                         IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                         int? commandTimeout = null) where T : class
    {
        return await GetRecordsInternalAsync<T>(storedProcedureName,
                                                CommandType.StoredProcedure,
                                                parameters,
                                                mappings,
                                                null,
                                                commandTimeout).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetRecordsAsync<T>(string storedProcedureName,
                                                         IEnumerable<IDbDataParameter> parameters,
                                                         IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                         IReadOnlyDictionary<string, Func<object, object>> converters,
                                                         int? commandTimeout = null) where T : class
    {
        return await GetRecordsInternalAsync<T>(storedProcedureName,
                                                CommandType.StoredProcedure,
                                                parameters,
                                                mappings,
                                                converters,
                                                commandTimeout).ConfigureAwait(false);
    }

    private async Task<IEnumerable<T>> GetRecordsInternalAsync<T>(string commandText,
                                                                  CommandType commandType,
                                                                  IEnumerable<IDbDataParameter>? parameters,
                                                                  IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                                  IReadOnlyDictionary<string, Func<object, object>> converters,
                                                                  int? commandTimeout = null) where T : class
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            throw new ArgumentNullException(nameof(commandText), "Command text cannot be null or empty");
        }

        if (mappings?.Any() != true)
        {
            throw new ArgumentException("Mapping cannot be null or empty");
        }

        List<T> models = new();

        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        using SqlCommand command = parameters == null
                                   ? CreateSqlCommand(connection, commandText, commandType, commandTimeout)
                                   : CreateSqlCommand(connection, commandText, commandType, parameters, commandTimeout);
        using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            if (converters?.Any() == true)
            {
                models.Add(ModelValuesMapper.MapTo<T>(mappings, reader, converters));
            }
            else
            {
                models.Add(ModelValuesMapper.MapTo<T>(mappings, reader, null));
            }
        }

        return models;
    }

    public async Task<IEnumerable<T>> GetRecordsByCmdTextAsync<T>(string commandText,
                                                                  IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                                  int? commandTimeout = null) where T : class
    {
        return await GetRecordsInternalAsync<T>(commandText,
                                                CommandType.Text,
                                                null,
                                                mappings,
                                                null,
                                                commandTimeout).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetRecordsByCmdTextAsync<T>(string commandText,
                                                                  IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                                  IReadOnlyDictionary<string, Func<object, object>> converters,
                                                                  int? commandTimeout = null) where T : class
    {
        return await GetRecordsInternalAsync<T>(commandText,
                                                CommandType.Text,
                                                null,
                                                mappings,
                                                converters,
                                                commandTimeout).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetRecordsByCmdTextAsync<T>(string commandText,
                                                                  IEnumerable<IDbDataParameter> parameters,
                                                                  IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                                  int? commandTimeout = null) where T : class
    {
        return await GetRecordsInternalAsync<T>(commandText,
                                                CommandType.Text,
                                                parameters,
                                                mappings,
                                                null,
                                                commandTimeout).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetRecordsByCmdTextAsync<T>(string commandText,
                                                                  IEnumerable<IDbDataParameter> parameters,
                                                                  IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                                  IReadOnlyDictionary<string, Func<object, object>> converters,
                                                                  int? commandTimeout = null) where T : class
    {
        return await GetRecordsInternalAsync<T>(commandText,
                                                CommandType.Text,
                                                parameters,
                                                mappings,
                                                converters,
                                                commandTimeout).ConfigureAwait(false);
    }
}
