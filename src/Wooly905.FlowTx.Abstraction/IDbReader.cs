using System;
using System.Collections.Generic;

namespace Wooly905.FlowTx.Abstraction;

public interface IDbReader
{
    int ColumnCount { get; }

    int RowCount { get; }

    bool HasRecord { get; }

    bool TryGetBoolValue(int columnIndex, int rowIndex, out bool? value);

    bool TryGetBoolValue(string columnName, int rowIndex, out bool? value);

    bool TryGetDataRowsValue(out IReadOnlyList<IReadOnlyDictionary<string, object>> value);

    bool TryGetDateTimeValue(int columnIndex, int rowIndex, out DateTime? value);

    bool TryGetDateTimeValue(string columnName, int rowIndex, out DateTime? value);

    bool TryGetInt64Value(int columnIndex, int rowIndex, out long? value);

    bool TryGetInt64Value(string columnName, int rowIndex, out long? value);

    bool TryGetIntValue(int columnIndex, int rowIndex, out int? value);

    bool TryGetIntValue(string columnName, int rowIndex, out int? value);

    bool TryGetIntValues(string columnName, out IReadOnlyList<int?> value);

    bool TryGetStringValue(int columnIndex, int rowIndex, out string value);

    bool TryGetStringValue(string columnName, int rowIndex, out string value);

    bool TryGetStringValue(int columnIndex, int rowIndex, out string columnName, out string value);

    bool TryGetDateTimeValues(string columnName, out IReadOnlyList<DateTime?> value);

    bool TryGetDecimalValue(string columnName, int rowIndex, out decimal? value);

    bool TryGetGuidValue(string columnName, int rowIndex, out Guid value);

    bool TryGetGuidValue(string columnName, int rowIndex, out Guid? value);
}
