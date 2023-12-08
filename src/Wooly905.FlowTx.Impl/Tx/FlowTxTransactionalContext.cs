using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Wooly905.FlowTx.Abstraction.Tx;

namespace Wooly905.FlowTx.Impl.Tx;

public class FlowTxTransactionalContext : IFlowTxTransactionalContext
{
    public async Task ExecuteCommandTextAsync(string commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            throw new ArgumentNullException("Command text could not be empty or null.");
        }

        if (FlowTxContextData.TransactionProvider.Value == null)
        {
            throw new InvalidOperationException("Unable to get transaction provider in current context");
        }

        IFlowTxProvider provider = FlowTxContextData.TransactionProvider.Value;
        await provider.ExecuteCommandTextAsync(commandText).ConfigureAwait(false);
    }

    public async Task ExecuteStoredProcedureAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters)
    {
        if (string.IsNullOrWhiteSpace(storedProcedureName))
        {
            throw new ArgumentNullException("Stored procedure name could not be empty or null.");
        }

        if (FlowTxContextData.TransactionProvider.Value == null)
        {
            throw new InvalidOperationException("Unable to get transaction provider in current context");
        }

        IFlowTxProvider provider = FlowTxContextData.TransactionProvider.Value;
        await provider.ExecuteStoredProcedureAsync(storedProcedureName, parameters).ConfigureAwait(false);
    }
}
