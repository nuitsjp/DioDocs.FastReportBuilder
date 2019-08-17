using System;
using GrapeCity.Documents.Excel;
using Newtonsoft.Json.Linq;

namespace DioDocs.FastReportBuilder
{
    public class TableRangeAccessor
    {
        private readonly string _name;
        private readonly JTokenAccessor _accessor;
        public int ColumnIndex { get; }

        public TableRangeAccessor(string name, string type, int columnIndex)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException($"name is null or empty");

            _name = name;
            _accessor = JTokenAccessor.GetConverter(type);
            ColumnIndex = columnIndex;
        }

        public void Set(ITableRow tableRow, JToken jToken)
        {
            tableRow.Range[ColumnIndex].Value = _accessor.Get(jToken[_name]);
        }
    }
}