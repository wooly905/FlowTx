using System.Data;
using Microsoft.Data.SqlClient;
using Wooly905.FlowTx.Impl;

namespace FlowTxTests.ManualTests;

public class StoredProcedureExecuteNonQueryTest
{
    [Fact]
    public async Task StoredProcedure_ExecuteNonQuery_WithParameter_Test()
    {
        FlowTxDbAccess dbAccess = new(TestConstants.GetConnectionString());

        List<IDbDataParameter> parameters =
        [
            new SqlParameter("FirstName", "John"),
            new SqlParameter("LastName", "Doe"),
            new SqlParameter("Title", "Mr."),
            new SqlParameter("TitleOfCourtesy", "Mr."),
            new SqlParameter("BirthDate", new DateTime(1980, 1, 1)),
            new SqlParameter("HireDate", new DateTime(2020, 1, 1)),
            new SqlParameter("Address", "123 Main St."),
            new SqlParameter("City", "New York"),
            new SqlParameter("Region", "NY"),
            new SqlParameter("PostalCode", "10001"),
            new SqlParameter("Country", "USA"),
            new SqlParameter("HomePhone", "123-456-7890"),
            new SqlParameter("Extension", "123"),
            new SqlParameter("Notes", "Test"),
            new SqlParameter("ReportsTo", 1),
            new SqlParameter("PhotoPath", "Test")
        ];

        int number = await dbAccess.ExecuteNonQueryAsync(TestConstants.StoredProcedureInsertEmployee(), parameters);

        Assert.Equal(1, number);
    }

    [Fact]
    public async void StoredProcedure_ExecuteNonQuery_WithoutParameter_Test()
    {
        FlowTxDbAccess dbAccess = new(TestConstants.GetConnectionString());
        int number = await dbAccess.ExecuteNonQueryAsync(TestConstants.StoredProcedureUpdateEmployeeOneExtension(), null);
        Assert.Equal(1, number);
    }
}
