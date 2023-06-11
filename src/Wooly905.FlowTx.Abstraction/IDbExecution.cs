using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Wooly905.FlowTx.Abstraction;

public interface IDbExecution
{
    Task ExecuteNonQueryAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters);

    Task ExecuteNonQueryAsync(string commandText);

    Task<object> ExecuteScalarAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters);

    Task<object> ExecuteScalarAsync(string commandText);

    Task<T> FetchRecordAsync<T>(string storedProcedureName,
                                IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings) where T : class;

    Task<T> FetchRecordAsync<T>(string storedProcedureName,
                                IEnumerable<IDbDataParameter> parameters,
                                IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings) where T : class;

    Task<T> FetchRecordAsync<T>(string storedProcedureName,
                                IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                IReadOnlyDictionary<string, Func<object, object>> convertFuncs) where T : class;

    Task<T> FetchRecordAsync<T>(string storedProcedureName,
                                IEnumerable<IDbDataParameter> parameters,
                                IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                IReadOnlyDictionary<string, Func<object, object>> convertFuncs) where T : class;

    Task<T> GetRecordAsync<T>(string commandText,
                              IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings) where T : class;

    Task<T> GetRecordAsync<T>(string commandText,
                              IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                              IReadOnlyDictionary<string, Func<object, object>> convertFuncs) where T : class;

    Task<IEnumerable<T>> FetchRecordsAsync<T>(string storedProcedureName) where T : class;

    Task<IEnumerable<T>> FetchRecordsAsync<T>(string storedProcedureName,
                                              IEnumerable<IDbDataParameter> parameters) where T : class;

    Task<IEnumerable<T>> FetchRecordsAsync<T>(string storedProcedureName,
                                              IEnumerable<IDbDataParameter> parameters,
                                              IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings) where T : class;

    Task<IEnumerable<T>> FetchRecordsAsync<T>(string storedProcedureName,
                                              IEnumerable<IDbDataParameter> parameters,
                                              IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                              IReadOnlyDictionary<string, Func<object, object>> funcs) where T : class;

    Task<IEnumerable<T>> GetRecordsAsync<T>(string commandText) where T : class;

    Task<IEnumerable<T>> GetRecordsAsync<T>(string commandText,
                                            IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings) where T : class;

    Task<IEnumerable<T>> GetRecordsAsync<T>(string commandText,
                                            IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                            IReadOnlyDictionary<string, Func<object, object>> funcs) where T : class;

    Task<IDbReader> GetDataReaderAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters);

    Task<IDbReader> GetDataReaderAsync(string commandText);
}
