using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Wooly905.FlowTx.Abstraction.Tx;

namespace Wooly905.FlowTx.Impl.Tx;

public class FlowTxTransactionManager : IFlowTxManager
{
    private readonly ConcurrentDictionary<Guid, (SqlConnection Connection, SqlTransaction Transaction, List<SqlCommand> Commands, DateTime CreatedTime)> _container;
    private readonly SemaphoreSlim _createSemaphore = new(1, 1);
    private readonly SemaphoreSlim _removeSemaphore = new(1, 1);
    private DateTime _lastExiprartionScanTime;
    private readonly string _dbConnectionString;

    public FlowTxTransactionManager(string connectionString)
    {
        _dbConnectionString = connectionString;
        _container = new ConcurrentDictionary<Guid, (SqlConnection, SqlTransaction, List<SqlCommand>, DateTime)>();
        _lastExiprartionScanTime = DateTime.Now;
    }

    public IDbCommand CreateCommand(Guid transactionId)
    {
        if (_container.TryGetValue(transactionId, out (SqlConnection Connection, SqlTransaction Transaction, List<SqlCommand> Commands, DateTime Now) obj))
        {
            SqlCommand command = obj.Connection.CreateCommand();
            command.Transaction = obj.Transaction;
            obj.Commands.Add(command);
            return command;
        }

        throw new ArgumentOutOfRangeException("Database transaction id is invalid.");
    }

    public Guid CreateTransaction(IsolationLevel level, bool isMultiActiveResultSet = false)
    {
        _createSemaphore.Wait();

        Guid guid = Guid.NewGuid();

        while (_container.ContainsKey(guid))
        {
            // This block ensures that we won't have duplicated GUID in the container.
            guid = Guid.NewGuid();
        }

        try
        {
            // determine MARS enabling for multi-tasking commands consideration
            SqlConnection connection = isMultiActiveResultSet
                                       ? new($"{_dbConnectionString};MultipleActiveResultSets=True")
                                       : new(_dbConnectionString);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction(level);
            List<SqlCommand> commands = new();
            _container[guid] = (connection, transaction, commands, DateTime.Now);

            return guid;
        }
        catch
        {
            throw;
        }
        finally
        {
            _createSemaphore.Release();
            StartScanForExpiredItems();
        }
    }

    private void StartScanForExpiredItems()
    {
        // Expiration time is 3 minutes. A normal transaction should not be run over 3 minutes.
        if (DateTime.Now.Subtract(_lastExiprartionScanTime).TotalSeconds > 180)
        {
            _lastExiprartionScanTime = DateTime.Now;
            Task.Factory.StartNew(state => RemoveExpiredItems((ConcurrentDictionary<Guid, (SqlConnection, SqlTransaction, List<SqlCommand>, DateTime)>)state),
                                  _container,
                                  CancellationToken.None,
                                  TaskCreationOptions.DenyChildAttach,
                                  TaskScheduler.Default);
        }
    }

    private static void RemoveExpiredItems(ConcurrentDictionary<Guid, (SqlConnection, SqlTransaction, List<SqlCommand>, DateTime)> container)
    {
        foreach (KeyValuePair<Guid, (SqlConnection Connection, SqlTransaction Transaction, List<SqlCommand> Commands, DateTime Now)> item in container)
        {
            if (DateTime.Now.Subtract(item.Value.Now).TotalSeconds > 180
                && container.TryRemove(item.Key, out (SqlConnection Connection, SqlTransaction Transaction, List<SqlCommand> Commands, DateTime Now) element))
            {
                foreach (SqlCommand command in element.Commands)
                {
                    command.Cancel();
                    command.Dispose();
                }

                if (element.Connection.State != ConnectionState.Closed)
                {
                    element.Connection.Close();
                }

                element.Transaction.Dispose();
                element.Connection.Dispose();
            }
        }
    }

    public IFlowTxExecution CreateTransactionExecution(Guid transactionId)
    {
        if (!_container.ContainsKey(transactionId))
        {
            throw new ArgumentOutOfRangeException("Database transaction id is invalid.");
        }

        return new FlowTxTransactionExecution(transactionId, this);
    }

    public IDbTransaction GetTransaction(Guid transactionId)
    {
        if (_container.TryGetValue(transactionId, out (SqlConnection Connection, SqlTransaction Transaction, List<SqlCommand> Commands, DateTime Now) obj))
        {
            return obj.Transaction;
        }

        throw new ArgumentOutOfRangeException("Database transaction id is invalid.");
    }

    public void RemoveTransaction(Guid transactionId)
    {
        if (!_container.ContainsKey(transactionId))
        {
            throw new ArgumentOutOfRangeException("Database transaction id is invalid.");
        }

        // Ensure that no more than 2 invocations with the same transaction id enter this function.
        _removeSemaphore.Wait();

        // If some item failed to get removed, expiration scan will take care of this.
        if (_container.TryRemove(transactionId, out (SqlConnection Connection, SqlTransaction Transaction, List<SqlCommand> Commands, DateTime Now) element))
        {
            foreach (SqlCommand command in element.Commands)
            {
                command.Cancel();
                command.Dispose();
            }

            if (element.Connection.State != ConnectionState.Closed)
            {
                element.Connection.Close();
            }

            element.Transaction.Dispose();
            element.Connection.Dispose();
        }

        _removeSemaphore.Release();
    }
}
