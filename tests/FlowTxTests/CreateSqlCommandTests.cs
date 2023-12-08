using Microsoft.Data.SqlClient;
using Wooly905.FlowTx.Impl;
using System.Data;

namespace FlowTxTests;

public class CreateSqlCommandTests
{
    [Fact]
    public void CreateSqlCommandTest()
    {
        SqlCommand cmd = FlowTxDbAccess.CreateSqlCommand(null, "select 1 from table", CommandType.Text, 100);

        Assert.NotNull(cmd);
        Assert.Equal("select 1 from table", cmd.CommandText);
        Assert.Equal(CommandType.Text, cmd.CommandType);
        Assert.Equal(100, cmd.CommandTimeout);
    }

    [Fact]
    public void CreateSqlCommandStoreProcedureTest()
    {
        SqlCommand cmd = FlowTxDbAccess.CreateSqlCommand(null, "usp_storedprocedure", CommandType.StoredProcedure, 100);

        Assert.NotNull(cmd);
        Assert.Equal("usp_storedprocedure", cmd.CommandText);
        Assert.Equal(CommandType.StoredProcedure, cmd.CommandType);
        Assert.Equal(100, cmd.CommandTimeout);
    }

    // test case for FlowTxDbAccess.CreateSqlCommand
    [Theory]
    [InlineData("select 1 from table", CommandType.Text, 100)]
    [InlineData("usp_storedprocedure", CommandType.StoredProcedure, 10)]
    public void CreateSqlCommandTestTheory(string commandText, CommandType commandType, int commandTimeout)
    {
        SqlCommand cmd = FlowTxDbAccess.CreateSqlCommand(null, commandText, commandType, commandTimeout);

        Assert.NotNull(cmd);
        Assert.Equal(commandText, cmd.CommandText);
        Assert.Equal(commandType, cmd.CommandType);
        Assert.Equal(commandTimeout, cmd.CommandTimeout);
    }

    // test case for FlowTxDbAccess.CreateSqlCommand with sql parameters 
    [Theory]
    [InlineData("usp_storedprocedure", CommandType.StoredProcedure, 10)]
    public void CreateSqlCommandWithParametersTestTheory(string commandText, CommandType commandType, int commandTimeout)
    {
        SqlParameter[] parameters = new SqlParameter[2];
        parameters[0] = new SqlParameter("@param1", SqlDbType.Int);
        parameters[0].Value = 1;
        parameters[1] = new SqlParameter("@param2", SqlDbType.VarChar);
        parameters[1].Value = "test";

        SqlCommand cmd = FlowTxDbAccess.CreateSqlCommand(null, commandText, commandType, parameters, commandTimeout);

        Assert.NotNull(cmd);
        Assert.Equal(commandText, cmd.CommandText);
        Assert.Equal(commandType, cmd.CommandType);
        Assert.Equal(commandTimeout, cmd.CommandTimeout);
        Assert.Equal(2, cmd.Parameters.Count);
        Assert.Equal("@param1", cmd.Parameters[0].ParameterName);
        Assert.Equal(SqlDbType.Int, cmd.Parameters[0].SqlDbType);
        Assert.Equal(1, cmd.Parameters[0].Value);
        Assert.Equal("@param2", cmd.Parameters[1].ParameterName);
        Assert.Equal(SqlDbType.VarChar, cmd.Parameters[1].SqlDbType);
        Assert.Equal("test", cmd.Parameters[1].Value);
    }
}