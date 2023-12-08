namespace FlowTxTests.ManualTests;

internal static class TestConstants
{
    public static string GetConnectionString()
    {
        return "Server=localhost;Database=Northwind;User Id=sa;Password=1qaz@WSX3edc;TrustServerCertificate=True";
    }

    public static string StoredProcedureInsertEmployee()
    {
        return "InsertEmployee";
    }

    public static string StoredProcedureGetEmployeesByFirstName()
    {
        return "GetEmployeesByFirstName";
    }

    public static string StoredProcedureGetAllEmployees()
    {
        return "GetAllEmployees";
    }

    public static string StoredProcedureGetAllOrders()
    {
        return "GetAllOrders";
    }

    public static string StoredProcedureUpdateEmployeeOneExtension()
    {
        return "UpdateEmployeeOneExtension";
    }
}
