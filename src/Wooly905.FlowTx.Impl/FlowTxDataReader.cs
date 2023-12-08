using System;
using System.Collections.Generic;
using System.Linq;
using Wooly905.FlowTx.Abstraction;

namespace Wooly905.FlowTx.Impl;

public class FlowTxDataReader : IFlowTxDbReader
{
    private readonly IReadOnlyList<string> _columnNames;
    private readonly IReadOnlyList<IReadOnlyDictionary<string, object>> _dataRows;

    public FlowTxDataReader(IReadOnlyList<string> columnNames, IReadOnlyList<IReadOnlyDictionary<string, object>> dataRows)
    {
        _columnNames = columnNames ?? throw new ArgumentNullException("column name list is null");
        _dataRows = dataRows ?? throw new ArgumentNullException("column row list is null");
    }

    public int ColumnCount => _dataRows.Count > 0 ? _dataRows[0].Count : 0;
    public int RowCount => _dataRows.Count;
    public bool HasRecord => RowCount > 0;

    public bool TryGetBoolValue(int columnIndex, int rowIndex, out bool? value)
    {
        if (TryGetValue(columnIndex, rowIndex, out object? outValue)
            && outValue is not null
            && bool.TryParse(outValue.ToString(), out bool value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetBoolValue(string columnName, int rowIndex, out bool? value)
    {
        if (TryGetValue(columnName, rowIndex, out object? outValue)
            && outValue is not null
            && bool.TryParse(outValue.ToString(), out bool value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetDataRowsValue(out IEnumerable<IReadOnlyDictionary<string, object>> value)
    {
        try
        {
            if (HasRecord)
            {
                value = _dataRows;
                return true;
            }
        }
        catch
        { }

        value = Enumerable.Empty<IReadOnlyDictionary<string, object>>();
        return false;
    }

    public bool TryGetDateTimeValue(int columnIndex, int rowIndex, out DateTime? value)
    {
        if (TryGetValue(columnIndex, rowIndex, out object? outValue)
            && outValue is not null
            && DateTime.TryParse(outValue.ToString(), out DateTime value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetDateTimeValue(string columnName, int rowIndex, out DateTime? value)
    {
        if (TryGetValue(columnName, rowIndex, out object? outValue)
            && outValue is not null
            && DateTime.TryParse(outValue.ToString(), out DateTime value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetDateTimeValues(string columnName, out IReadOnlyList<DateTime?> model)
    {
        List<DateTime?> tmp = new();

        for (int i = 0; i < RowCount; i++)
        {
            if (!TryGetValue(columnName, i, out object? outValue)
                || outValue is not DateTime dateValue)
            {
                model = tmp;
                return false;
            }

            tmp.Add(dateValue);
        }

        model = tmp;
        return true;
    }

    public bool TryGetDecimalValue(string columnName, int rowIndex, out decimal? value)
    {
        if (TryGetValue(columnName, rowIndex, out object? outValue)
            && outValue is not null)
        {
            if (outValue is DBNull)
            {
                value = null;
                return true;
            }
            else if (decimal.TryParse(outValue.ToString(), out decimal value2))
            {
                value = value2;
                return true;
            }
        }

        value = null;
        return false;
    }

    public bool TryGetGuidValue(string columnName, int rowIndex, out Guid value)
    {
        if (TryGetValue(columnName, rowIndex, out object? outValue)
            && outValue is not null)
        {
            if (outValue is DBNull)
            {
                value = Guid.Empty;
                return false;
            }
            else if (Guid.TryParse(outValue.ToString(), out Guid value2))
            {
                value = value2;
                return true;
            }
        }

        value = Guid.Empty;
        return false;
    }

    public bool TryGetGuidValue(string columnName, int rowIndex, out Guid? value)
    {
        if (TryGetValue(columnName, rowIndex, out object? outValue)
            && outValue is not null)
        {
            if (outValue is DBNull)
            {
                value = null;
                return true;
            }
            else if (Guid.TryParse(outValue.ToString(), out Guid value2))
            {
                value = value2;
                return true;
            }
        }

        value = null;
        return false;
    }

    public bool TryGetInt64Value(int columnIndex, int rowIndex, out long? value)
    {
        if (TryGetValue(columnIndex, rowIndex, out object? outValue)
            && outValue is not null
            && long.TryParse(outValue.ToString(), out long value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetInt64Value(string columnName, int rowIndex, out long? value)
    {
        if (TryGetValue(columnName, rowIndex, out object? outValue)
            && outValue is not null
            && long.TryParse(outValue.ToString(), out long value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetIntValue(int columnIndex, int rowIndex, out int? value)
    {
        if (TryGetValue(columnIndex, rowIndex, out object? outValue)
            && outValue is not null
            && int.TryParse(outValue.ToString(), out int value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetIntValue(string columnName, int rowIndex, out int? value)
    {
        if (TryGetValue(columnName, rowIndex, out object? outValue)
            && outValue is not null
            && int.TryParse(outValue.ToString(), out int value2))
        {
            value = value2;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetIntValues(string columnName, out IReadOnlyList<int?> model)
    {
        List<int?> tmp = new();

        for (int i = 0; i < RowCount; i++)
        {
            if (!TryGetValue(columnName, i, out object? outValue)
                || outValue is null
                || !int.TryParse(outValue.ToString(), out int intValue))
            {
                model = tmp;
                return false;
            }

            tmp.Add(intValue);
        }

        model = tmp;
        return true;
    }

    public bool TryGetStringValue(int columnIndex, int rowIndex, out string value)
    {
        if (TryGetValue(columnIndex, rowIndex, out object? outValue)
            && outValue is not null)
        {
            value = outValue is null ? string.Empty : outValue.ToString();
            return true;
        }

        value = string.Empty;
        return false;
    }

    public bool TryGetStringValue(string columnName, int rowIndex, out string value)
    {
        if (TryGetValue(columnName, rowIndex, out object? outValue)
            && outValue is not null)
        {
            value = outValue is null ? string.Empty : outValue.ToString();
            return true;
        }

        value = string.Empty;
        return false;
    }

    public bool TryGetStringValue(int columnIndex, int rowIndex, out string columnName, out string value)
    {
        if (TryGetValue(columnIndex, rowIndex, out string outColumnName, out object? outValue)
            && outValue is not null)
        {
            columnName = outColumnName;
            value = outValue is null ? string.Empty : outValue.ToString();
            return true;
        }

        value = string.Empty;
        columnName = string.Empty;

        return false;
    }

    private bool TryGetValue(int columnIndex, int rowIndex, out object? value)
    {
        try
        {
            if (HasRecord)
            {
                string columnName = _columnNames[columnIndex];
                value = _dataRows[rowIndex][columnName];
                return true;
            }
        }
        catch
        { }

        value = default;
        return false;
    }

    private bool TryGetValue(string columnName, int rowIndex, out object? value)
    {
        try
        {
            if (HasRecord)
            {
                value = _dataRows[rowIndex][columnName];
                return true;
            }
        }
        catch
        { }

        value = default;
        return false;
    }

    private bool TryGetValue(int columnIndex, int rowIndex, out string columnName, out object? value)
    {
        try
        {
            if (HasRecord)
            {
                columnName = _columnNames[columnIndex];
                value = _dataRows[rowIndex][columnName];
                return true;
            }
        }
        catch
        { }

        value = default;
        columnName = string.Empty;
        return false;
    }
}
