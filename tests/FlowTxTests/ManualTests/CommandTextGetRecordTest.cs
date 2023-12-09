using Wooly905.FlowTx.Impl;

namespace FlowTxTests.ManualTests;

public class CommandTextGetRecordTest
{
    [Fact]
    public async Task CommandText_GetSingleRecord_TestAsync()
    {
        FlowTxDbAccess db = new(TestConstants.GetConnectionString());

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

        Order? order = await db.GetRecordByCmdTextAsync<Order>("SELECT * FROM Orders WHERE OrderID = 10248", mapping);

        Assert.NotNull(order);
        Assert.Equal(10248, order.OrderID);
        Assert.Equal("VINET", order.CustomerID);
        Assert.Equal(5, order.EmployeeID);
        Assert.Equal(new DateTime(1996, 7, 4), order.OrderDate);
    }

    // command text get single record with converter

    [Fact]
    public async Task CommandText_GetMultiRecords_TestAsync()
    {
        FlowTxDbAccess db = new(TestConstants.GetConnectionString());

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

        IEnumerable<Order> orders = await db.GetRecordsByCmdTextAsync<Order>("SELECT * FROM Orders WHERE EmployeeId = 5", mapping);

        Assert.NotNull(orders);
        Assert.Equal(42, orders.Count());
    }

    // command text get multi records with converter
}
