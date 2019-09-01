using System;
using GrapeCity.Documents.Excel;
using Newtonsoft.Json.Linq;

namespace DioDocs.FastReportBuilder
{
    internal class RangeAccessor
    {
        private readonly string _name;
        private readonly string _cell;
        private readonly JTokenAccessor _accessor;

        internal RangeAccessor(string name, string type, string cell)
        {
            _name = name;
            _accessor = JTokenAccessor.GetConverter(type);
            _cell = cell;
        }

        internal void Set(IWorksheet worksheet, JToken jObject)
        {
            worksheet.Range[_cell].Value = _accessor.Get(jObject[_name]);
        }
    }
}