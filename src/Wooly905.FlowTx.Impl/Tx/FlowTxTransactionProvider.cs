using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Wooly905.FlowTx.Abstraction.Tx;

namespace Wooly905.FlowTx.Impl.Tx;

internal class FlowTxTransactionProvider : IFlowTxProvider, IDisposable
{
    private readonly IFlowTxManager _manager;
    private Guid _transactionId;
    private readonly IFlowTxExecution _execution;

    public FlowTxTransactionProvider(IFlowTxManager manager, bool isMultiActiveResultSet = false)
    {
        _manager = manager ?? throw new ArgumentNullException("Transaction manager could not be null.");
        _transactionId = _manager.CreateTransaction(IsolationLevel.Serializable, isMultiActiveResultSet);
        _execution = _manager.CreateTransactionExecution(_transactionId);
    }

    public void CommitTransaction()
    {
        _execution.CommitTransaction();
    }

    public void Dispose()
    {
        _manager.RemoveTransaction(_transactionId);
    }

    public async Task ExecuteCommandTextAsync(string commandText)
    {
        await _execution.ExecuteCommandTextAsync(commandText).ConfigureAwait(false);
    }

    public async Task ExecuteStoredProcedureAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters)
    {
        await _execution.ExecuteStoreProcedureAsync(storedProcedureName, parameters).ConfigureAwait(false);
    }

    public void RollbackTransaction()
    {
        _execution.RollbackTransaction();
    }
}
