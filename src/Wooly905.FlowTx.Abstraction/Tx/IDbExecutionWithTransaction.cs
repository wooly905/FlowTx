using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Wooly905.FlowTx.Abstraction.Tx;

public interface IDbExecutionWithTransaction
{
    Task ExecuteStoredProcedureAsync(string storedProcedureName, IReadOnlyList<IDbDataParameter> parameters);

    Task ExecuteCommandTextAsync(string commandText);
}
