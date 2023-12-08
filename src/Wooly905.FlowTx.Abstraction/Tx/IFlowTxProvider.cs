using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Wooly905.FlowTx.Abstraction.Tx;

public interface IFlowTxProvider
{
    void CommitTransaction();

    void Dispose();

    Task ExecuteStoredProcedureAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters);

    Task ExecuteCommandTextAsync(string commandText);

    void RollbackTransaction();
}
