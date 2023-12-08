using System;
using Wooly905.FlowTx.Abstraction.Tx;

namespace Wooly905.FlowTx.Impl.Tx;

// TODO - should have an interface design
public class FlowTxScope
{
    public bool IsComplete { get; private set; }

    public FlowTxScope(IFlowTxManager manager, bool supportMultiTasking = false)
    {
        FlowTxContextData.TransactionProvider.Value = new FlowTxTransactionProvider(manager, supportMultiTasking);
    }

    public void Complete()
    {
        if (FlowTxContextData.TransactionProvider.Value == null)
        {
            throw new InvalidOperationException("Unable to get transaction provider in current context.");
        }

        FlowTxContextData.TransactionProvider.Value.CommitTransaction();
        IsComplete = true;
    }

    public void Dispose()
    {
        if (FlowTxContextData.TransactionProvider.Value == null)
        {
            throw new InvalidOperationException("Unable to get transaction provider in current context.");
        }

        if (!IsComplete)
        {
            FlowTxContextData.TransactionProvider.Value.RollbackTransaction();
        }

        FlowTxContextData.TransactionProvider.Value.Dispose();
    }
}
