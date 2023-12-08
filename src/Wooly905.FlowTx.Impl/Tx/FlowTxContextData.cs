using System.Threading;
using Wooly905.FlowTx.Abstraction.Tx;

namespace Wooly905.FlowTx.Impl.Tx;

internal static class FlowTxContextData
{
    public static readonly AsyncLocal<IFlowTxProvider> TransactionProvider = new();
}
