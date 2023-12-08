using System;
using System.Data;

namespace Wooly905.FlowTx.Abstraction.Tx;

public interface IFlowTxManager
{
    Guid CreateTransaction(IsolationLevel level, bool isMultiActiveResultSet = false);

    IFlowTxExecution CreateTransactionExecution(Guid transactionId);

    IDbCommand CreateCommand(Guid transactionId);

    IDbTransaction GetTransaction(Guid transactionId);

    void RemoveTransaction(Guid transactionId);
}
