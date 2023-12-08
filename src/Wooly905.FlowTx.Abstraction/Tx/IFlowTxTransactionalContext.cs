using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Wooly905.FlowTx.Abstraction.Tx;

public interface IFlowTxTransactionalContext
{
    Task ExecuteStoredProcedureAsync(string storedProcedureName, IEnumerable<IDbDataParameter> parameters);

    Task ExecuteCommandTextAsync(string commandText);
}
