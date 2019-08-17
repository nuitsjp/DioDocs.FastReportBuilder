using System;
using GrapeCity.Documents.Excel;
using Newtonsoft.Json.Linq;

namespace DioDocs.FastReportBuilder
{
    public class RangeAccessor
    {
        private readonly string _name;
        private readonly string _cell;
        private readonly JTokenAccessor _accessor;

        public RangeAccessor(string name, string type, string cell)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException($"name is null or empty");
            _name = name;
            _accessor = JTokenAccessor.GetConverter(type);
            _cell = cell;

        }

        public void Set(IWorksheet worksheet, JToken jObject)
        {
            worksheet.Range[_cell].Value = _accessor.Get(jObject[_name]);
        }
    }
}