using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using GrapeCity.Documents.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DioDocs.FastReportBuilder
{
    public class JsonReportBuilder : IJsonReportBuilder
    {
        private readonly byte[] _template;
        private bool _isInitialized;
        /// <summary>
        /// 単項目を設定するためのSetter
        /// </summary>
        private readonly List<RangeAccessor> _accessors = new List<RangeAccessor>();

        /// <summary>
        /// テーブル名
        /// </summary>
        private string _tableName;

        /// <summary>
        /// テーブルの列項目を設定するためのSetter
        /// </summary>
        private readonly List<TableRangeAccessor> _tableAccessors = new List<TableRangeAccessor>();

        public JsonReportBuilder(Stream template)
        {
            _template = new byte[template.Length];
            template.Read(_template, 0, _template.Length);
        }

        public void Build(TextReader input, Stream output, SaveFileFormat saveFileFormat)
        {
            using (var memoryStream = new MemoryStream(_template))
            using (var reader = new JsonTextReader(input))
            {
                IWorkbook workbook = new Workbook();
                workbook.Open(memoryStream);

                var settingWorksheet = workbook.Worksheets["DioDocs.FastReportBuilder"];
                if (settingWorksheet == null) throw new InvalidOperationException("Setting Worksheet(DioDocs.FastReportBuilder) is not exist.");

                if (!_isInitialized) ParseSettings(settingWorksheet);

                settingWorksheet.Delete();


                var worksheet = workbook.Worksheets[0];
                var json = JToken.ReadFrom(reader);

                foreach (var rangeAccessor in _accessors)
                {
                    rangeAccessor.Set(worksheet, json);
                }

                if (_tableName != null)
                    SetTable(worksheet, json);

                workbook.Save(output, (GrapeCity.Documents.Excel.SaveFileFormat)saveFileFormat);
            }
        }

        private void SetTable(IWorksheet worksheet, JToken json)
        {
            var table = worksheet.Tables[_tableName];
            var rows = json[_tableName];

            // テーブルの行数を確認し、不足分を追加する
            if (table.Rows.Count < rows.Count())
            {
                var addCount = rows.Count() - table.Rows.Count;
                for (var i = 0; i < addCount; i++)
                {
                    table.Rows.Add(table.Rows.Count - 1);
                }
            }

            // テーブルに値を設定する
            var rowNumber = 0;
            foreach (var row in rows)
            {
                var tableRow = table.Rows[rowNumber];
                foreach (var tableAccessor in _tableAccessors)
                {
                    tableAccessor.Set(tableRow, row);
                }

                rowNumber++;
            }
        }

        private void ParseSettings(IWorksheet settingWorksheet)
        {
            var usedRange = settingWorksheet.UsedRange;
            for (var i = 1; i < usedRange.Rows.Count; i++)
            {
                var name = usedRange[i, 0].Value.ToString();
                var type = usedRange[i, 1].Value.ToString();

                if (_tableName is null)
                {
                    if (type == "table")
                    {
                        _tableName = name;
                    }
                    else
                    {
                        _accessors.Add(new RangeAccessor(name, type, usedRange[i, 2].Value.ToString()));
                    }
                }
                else
                {
                    _tableAccessors.Add(new TableRangeAccessor(name, type, int.Parse(usedRange[i, 3].Value.ToString())));
                }
            }
            _isInitialized = true;
        }
    }
}