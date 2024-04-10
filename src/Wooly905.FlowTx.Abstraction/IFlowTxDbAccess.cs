using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Wooly905.FlowTx.Abstraction;

public interface IFlowTxDbAccess
{
    Task<int> ExecuteNonQueryAsync(string storedProcedureName,
                                   int? commandTimeout = null);

    Task<int> ExecuteNonQueryAsync(string storedProcedureName,
                                   IEnumerable<IDbDataParameter> parameters,
                                   int? commandTimeout = null);

    Task<int>ExecuteNonQueryByCmdTextAsync(string commandText,
                                           int? commandTimeout = null);

    Task<int> ExecuteNonQueryByCmdTextAsync(string commandText,
                                            IEnumerable<IDbDataParameter> parameters,
                                            int? commandTimeout = null);

    Task<object> ExecuteScalarAsync(string storedProcedureName,
                                    int? commandTimeout = null);

    Task<object> ExecuteScalarAsync(string storedProcedureName,
                                    IEnumerable<IDbDataParameter> parameters,
                                    int? commandTimeout = null);

    Task<object> ExecuteScalarByCmdTextAsync(string commandText,
                                             int? commandTimeout = null);

    Task<T?> GetRecordAsync<T>(string storedProcedureName,
                               IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                               int? commandTimeout = null) where T : class;

    Task<T?> GetRecordAsync<T>(string storedProcedureName,
                               IEnumerable<IDbDataParameter> parameters,
                               IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                               int? commandTimeout = null) where T : class;

    Task<T?> GetRecordAsync<T>(string storedProcedureName,
                               IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                               IReadOnlyDictionary<string, Func<object, object>> converters,
                               int? commandTimeout = null) where T : class;

    Task<T?> GetRecordAsync<T>(string storedProcedureName,
                               IEnumerable<IDbDataParameter> parameters,
                               IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                               IReadOnlyDictionary<string, Func<object, object>> converters,
                               int? commandTimeout = null) where T : class;

    Task<T?> GetRecordByCmdTextAsync<T>(string commandText,
                                        IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                        int? commandTimeout = null) where T : class;

    Task<T?> GetRecordByCmdTextAsync<T>(string commandText,
                                        IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                        IReadOnlyDictionary<string, Func<object, object>> converters,
                                        int? commandTimeout = null) where T : class;

    Task<IEnumerable<T>> GetRecordsAsync<T>(string storedProcedureName,
                                            IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                            int? commandTimeout = null) where T : class;

    Task<IEnumerable<T>> GetRecordsAsync<T>(string storedProcedureName,
                                            IEnumerable<IDbDataParameter> parameters,
                                            IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                            int? commandTimeout = null) where T : class;

    Task<IEnumerable<T>> GetRecordsAsync<T>(string storedProcedureName,
                                            IEnumerable<IDbDataParameter> parameters,
                                            IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                            IReadOnlyDictionary<string, Func<object, object>> converters,
                                            int? commandTimeout = null) where T : class;

    Task<IEnumerable<T>> GetRecordsByCmdTextAsync<T>(string commandText,
                                                     IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                     int? commandTimeout = null) where T : class;

    Task<IEnumerable<T>> GetRecordsByCmdTextAsync<T>(string commandText,
                                                     IEnumerable<IDbDataParameter> parameters,
                                                     IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                     int? commandTimeout = null) where T : class;

    Task<IEnumerable<T>> GetRecordsByCmdTextAsync<T>(string commandText,
                                                     IEnumerable<IDbDataParameter> parameters,
                                                     IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                     IReadOnlyDictionary<string, Func<object, object>> converters,
                                                     int? commandTimeout = null) where T : class;

    Task<IEnumerable<T>> GetRecordsByCmdTextAsync<T>(string commandText,
                                                     IEnumerable<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mappings,
                                                     IReadOnlyDictionary<string, Func<object, object>> converters,
                                                     int? commandTimeout = null) where T : class;

    Task<IFlowTxDbReader> GetDataReaderAsync(string storedProcedureName,
                                             IEnumerable<IDbDataParameter> parameters,
                                             int? commandTimeout = null);

    Task<IFlowTxDbReader> GetDataReaderAsync(string commandText,
                                             int? commandTimeout = null);
}
