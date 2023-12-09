using System.Data;
using Microsoft.Data.SqlClient;
using Wooly905.FlowTx.Impl;

namespace FlowTxTests.ManualTests;

public class StoredProcedureGetRecordTest
{
    [Fact]
    public async Task StoredProcedure_GetSingleRecord_WithParameter_Test()
    {
        FlowTxDbAccess dbAccess = new(TestConstants.GetConnectionString());

        List<IDbDataParameter> parameters = new()
        {
            new SqlParameter("FirstName", "Janet")  // Janet is the employee who has EmployeeId = 3
        };

        List<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mapping = new()
        {
            ("EmployeeId", "EmployeeId", typeof(int)),
            ("FirstName", "FirstName", typeof(string)),
            ("LastName", "LastName", typeof(string)),
            ("Title", "Title", typeof(string)),
            ("TitleOfCourtesy", "TitleOfCourtesy", typeof(string)),
            ("BirthDate", "BirthDate", typeof(DateTime)),
            ("HireDate", "HireDate", typeof(DateTime)),
            ("Address", "Address", typeof(string)),
            ("City", "City", typeof(string)),
            ("Region", "Region", typeof(string)),
            ("PostalCode", "PostalCode", typeof(string)),
            ("Country", "Country", typeof(string)),
            ("HomePhone", "HomePhone", typeof(string)),
            ("Extension", "Extension", typeof(string)),
            ("Notes", "Notes", typeof(string)),
            ("ReportsTo", "ReportsTo", typeof(int)),
            ("PhotoPath", "PhotoPath", typeof(string))
        };

        Employee? employee = await dbAccess.GetRecordAsync<Employee>(TestConstants.StoredProcedureGetEmployeesByFirstName(), parameters, mapping);

        Assert.NotNull(employee);
        Assert.Equal(3, employee.EmployeeID);
        Assert.Equal("Janet", employee.FirstName);
    }

    // stored procedure get single record with parameter with converter

    [Fact]
    public async Task StoredProcedure_GetSingleRecord_WithoutParameter_Test()
    {
        FlowTxDbAccess dbAccess = new(TestConstants.GetConnectionString());

        List<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mapping = new()
        {
            ("EmployeeId", "EmployeeId", typeof(int)),
            ("FirstName", "FirstName", typeof(string)),
            ("LastName", "LastName", typeof(string)),
            ("Title", "Title", typeof(string)),
            ("TitleOfCourtesy", "TitleOfCourtesy", typeof(string)),
            ("BirthDate", "BirthDate", typeof(DateTime)),
            ("HireDate", "HireDate", typeof(DateTime)),
            ("Address", "Address", typeof(string)),
            ("City", "City", typeof(string)),
            ("Region", "Region", typeof(string)),
            ("PostalCode", "PostalCode", typeof(string)),
            ("Country", "Country", typeof(string)),
            ("HomePhone", "HomePhone", typeof(string)),
            ("Extension", "Extension", typeof(string)),
            ("Notes", "Notes", typeof(string)),
            ("ReportsTo", "ReportsTo", typeof(int)),
            ("PhotoPath", "PhotoPath", typeof(string))
        };

        Employee? employee = await dbAccess.GetRecordAsync<Employee>(TestConstants.StoredProcedureGetAllEmployees(), mapping);
        // Nancy is the employee who has EmployeeId = 1, so the record should be on the top in the result set
        Assert.NotNull(employee);
        Assert.Equal(1, employee.EmployeeID);
        Assert.Equal("Nancy", employee.FirstName);
    }

    // stored procedure get single record without parameter with converter

    [Fact]
    public async Task StoredProcedure_GetMultiRecords_WithParameter_Test()
    {
        FlowTxDbAccess dbAccess = new(TestConstants.GetConnectionString());

        List<IDbDataParameter> parameters = new()
        {
            new SqlParameter("FirstName", "M")  // Janet is the employee who has EmployeeId = 3
        };

        List<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mapping = new()
        {
            ("EmployeeId", "EmployeeId", typeof(int)),
            ("FirstName", "FirstName", typeof(string)),
            ("LastName", "LastName", typeof(string)),
            ("Title", "Title", typeof(string)),
            ("TitleOfCourtesy", "TitleOfCourtesy", typeof(string)),
            ("BirthDate", "BirthDate", typeof(DateTime)),
            ("HireDate", "HireDate", typeof(DateTime)),
            ("Address", "Address", typeof(string)),
            ("City", "City", typeof(string)),
            ("Region", "Region", typeof(string)),
            ("PostalCode", "PostalCode", typeof(string)),
            ("Country", "Country", typeof(string)),
            ("HomePhone", "HomePhone", typeof(string)),
            ("Extension", "Extension", typeof(string)),
            ("Notes", "Notes", typeof(string)),
            ("ReportsTo", "ReportsTo", typeof(int)),
            ("PhotoPath", "PhotoPath", typeof(string))
        };

        IEnumerable<Employee> employees = await dbAccess.GetRecordsAsync<Employee>(TestConstants.StoredProcedureGetEmployeesByFirstName(), parameters, mapping);

        Assert.NotNull(employees);
        Assert.Equal(2, employees.Count()); // Margaret and Michael
        Assert.Equal(4, employees.First().EmployeeID);
        Assert.Equal("Margaret", employees.First().FirstName);
        Assert.Equal("Peacock", employees.First().LastName);
        Assert.Equal("Sales Representative", employees.First().Title);
        Assert.Equal("Mrs.", employees.First().TitleOfCourtesy);
        Assert.Equal(new DateTime(1937, 9, 19), employees.First().BirthDate);
        Assert.Equal(new DateTime(1993, 5, 3), employees.First().HireDate);
        Assert.Equal("4110 Old Redmond Rd.", employees.First().Address);
        Assert.Equal("Redmond", employees.First().City);
        Assert.Equal("WA", employees.First().Region);
        Assert.Equal("98052", employees.First().PostalCode);
        Assert.Equal("USA", employees.First().Country);
        Assert.Equal("(206) 555-8122", employees.First().HomePhone);
        Assert.Equal("5176", employees.First().Extension);
        Assert.Equal("Margaret holds a BA in English literature from Concordia College (1958) and an MA from the American Institute of Culinary Arts (1966).  She was assigned to the London office temporarily from July through November 1992.", employees.First().Notes);
        Assert.Equal(2, employees.First().ReportsTo);
        Assert.Equal("http://accweb/emmployees/peacock.bmp", employees.First().PhotoPath);

        List<IDbDataParameter> parameters2 = new()
        {
            new SqlParameter("FirstName", "Z")  // There doesn't exist any employee whose first name starts with Z
        };

        IEnumerable<Employee> employees2 = await dbAccess.GetRecordsAsync<Employee>(TestConstants.StoredProcedureGetEmployeesByFirstName(), parameters2, mapping);

        Assert.NotNull(employees2);
        Assert.Empty(employees2);
    }

    // stored procedure get multi records with parameter with converter

    [Fact]
    public async Task StoredProcedure_GetMultiRecords_WithoutParameter_Test()
    {
        FlowTxDbAccess dbAccess = new(TestConstants.GetConnectionString());

        // generate order mapping 
        List<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mapping = new()
        {
            ("OrderID", "OrderID", typeof(int)),
            ("CustomerID", "CustomerID", typeof(string)),
            ("EmployeeID", "EmployeeID", typeof(int)),
            ("OrderDate", "OrderDate", typeof(DateTime)),
            ("RequiredDate", "RequiredDate", typeof(DateTime)),
            ("ShippedDate", "ShippedDate", typeof(DateTime)),
            ("ShipVia", "ShipVia", typeof(int)),
            ("Freight", "Freight", typeof(decimal)),
            ("ShipName", "ShipName", typeof(string)),
            ("ShipAddress", "ShipAddress", typeof(string)),
            ("ShipCity", "ShipCity", typeof(string)),
            ("ShipRegion", "ShipRegion", typeof(string)),
            ("ShipPostalCode", "ShipPostalCode", typeof(string)),
            ("ShipCountry", "ShipCountry", typeof(string))
        };

        IEnumerable<Order> orders = await dbAccess.GetRecordsAsync<Order>(TestConstants.StoredProcedureGetAllOrders(), mapping);

        Assert.NotNull(orders);
        Assert.Equal(830, orders.Count());
        Assert.Equal(10248, orders.First().OrderID);
        Assert.Equal("VINET", orders.First().CustomerID);
        Assert.Equal(5, orders.First().EmployeeID);
    }

    // stored procedure get multi records without parameter with converter
}
