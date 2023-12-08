using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Wooly905.FlowTx.Abstraction.Tx;

namespace Wooly905.FlowTx.Impl.Tx;

public class FlowTxTransactionExecution : IFlowTxExecution
{
    private readonly Guid _transactionId;
    private readonly IFlowTxManager _manager;

    public FlowTxTransactionExecution(Guid transactionId, IFlowTxManager manager)
    {
        _transactionId = transactionId;
        _manager = manager ?? throw new ArgumentNullException("IPulsarDatabaseTransactionManager is null.");
    }

    public void CommitTransaction()
    {
        IDbTransaction trans = _manager.GetTransaction(_transactionId);

        if (trans == null)
        {
            throw new InvalidOperationException("Cannot get transaction object to work.");
        }

        trans.Commit();
    }

    public async Task ExecuteCommandTextAsync(string commandText)
    {
        IDbCommand command = _manager.CreateCommand(_transactionId);

        if (command is not SqlCommand sqlCommand)
        {
            throw new InvalidCastException("Wrong type of transaction command.");
        }

        command.CommandType = CommandType.Text;
        command.CommandText = commandText;
        command.CommandTimeout = FlowTxExtensions.CommandTimeout;

        await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task ExecuteStoreProcedureAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters)
    {
        IDbCommand command = _manager.CreateCommand(_transactionId);

        if (command is not SqlCommand sqlCommand)
        {
            throw new InvalidCastException("Wrong type of transaction command.");
        }

        command.PrepareParameters(parameters);
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = storedProcedureName;
        command.CommandTimeout = FlowTxExtensions.CommandTimeout;

        await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public void RollbackTransaction()
    {
        IDbTransaction trans = _manager.GetTransaction(_transactionId) ?? throw new InvalidOperationException("Could not get transaction object.");
        trans.Rollback();
    }
}
