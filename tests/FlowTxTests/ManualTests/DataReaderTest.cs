using Wooly905.FlowTx.Impl;
using Wooly905.FlowTx.Abstraction;

namespace FlowTxTests.ManualTests;

public class DataReaderTest
{
    [Fact]
    public async Task DataReader_GetRecordsByCmdText_Test()
    {
        FlowTxDbAccess dbAccess = new(TestConstants.GetConnectionString());

        List<(string ModelPropertyPath, string ColumnName, Type ModelPropertyType)> mapping = new()
        {
            ("OrderID", "OrderID", typeof(int)),
            ("CustomerID", "CustomerID", typeof(string)),
            ("EmployeeID", "EmployeeID", typeof(int?)),
            ("OrderDate", "OrderDate", typeof(DateTime?)),
            ("RequiredDate", "RequiredDate", typeof(DateTime?)),
            ("ShippedDate", "ShippedDate", typeof(DateTime?)),
            ("ShipVia", "ShipVia", typeof(int?)),
            ("Freight", "Freight", typeof(decimal?)),
            ("ShipName", "ShipName", typeof(string)),
            ("ShipAddress", "ShipAddress", typeof(string)),
            ("ShipCity", "ShipCity", typeof(string)),
            ("ShipRegion", "ShipRegion", typeof(string)),
            ("ShipPostalCode", "ShipPostalCode", typeof(string)),
            ("ShipCountry", "ShipCountry", typeof(string))
        };

        IFlowTxDbReader reader = await dbAccess.GetDataReaderAsync("SELECT * FROM Orders");

        Assert.NotNull(reader);
        Assert.True(reader.HasRecord);
        Assert.Equal(830, reader.RowCount);
        Assert.Equal(14, reader.ColumnCount);
    }

    //[Fact]
    //public async Task DataReader_GetRecords_WithConverters_Test()
    //{
    //    FlowTxDbAccess dbAccess = new(TestConstants.GetConnectionString());

       
    //}
}
