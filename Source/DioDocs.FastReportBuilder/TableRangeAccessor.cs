using System;
using GrapeCity.Documents.Excel;
using Newtonsoft.Json.Linq;

namespace DioDocs.FastReportBuilder
{
    internal class TableRangeAccessor
    {
        private readonly string _name;
        private readonly JTokenAccessor _accessor;
        internal int ColumnIndex { get; }

        internal TableRangeAccessor(string name, string type, int columnIndex)
        {
            _name = name;
            _accessor = JTokenAccessor.GetConverter(type);
            ColumnIndex = columnIndex;
        }

        internal void Set(ITableRow tableRow, JToken jToken)
        {
            tableRow.Range[ColumnIndex].Value = _accessor.Get(jToken[_name]);
        }
    }
}