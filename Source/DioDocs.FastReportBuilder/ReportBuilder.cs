using System;
using System.Collections.Generic;
using System.IO;
using GrapeCity.Documents.Excel;

namespace DioDocs.FastReportBuilder
{
    public class ReportBuilder<TReportRow> : IReportBuilder<TReportRow>
    {
        /// <summary>
        /// テンプレートとなるExcelファイル
        /// </summary>
        private readonly byte[] _template;
        /// <summary>
        /// 明細行を表示するExcelテーブルの名称
        /// </summary>
        private readonly string _tableName;

        /// <summary>
        /// 単項目を設定するためのSetter
        /// </summary>
        private readonly Dictionary<object, Action<IRange>> _setters = 
            new Dictionary<object, Action<IRange>>();
        /// <summary>
        /// テーブルの列項目を設定するためのSetter
        /// </summary>
        private readonly Dictionary<object, Action<IRange, TReportRow>> _tableSetters = 
            new Dictionary<object, Action<IRange, TReportRow>>();

        public ReportBuilder(Stream template)
        {
            _template = new byte[template.Length];
            template.Read(_template, 0, (int)template.Length);
            _tableName = typeof(TReportRow).Name;
        }

        public IReportBuilder<TReportRow> AddSetter(object key, Action<IRange> setter)
        {
            _setters[key] = setter;
            return this;
        }

        public IReportBuilder<TReportRow> AddTableSetter(string key, Action<IRange, TReportRow> setter)
        {
            _tableSetters[key] = setter;
            return this;
        }

        public void Build(IList<TReportRow> rows, Stream stream, SaveFileFormat saveFileFormat)
        {
            IWorkbook workbook;
            using (var inputStream = new MemoryStream(_template))
            {
                workbook = new Workbook();
                workbook.Open(inputStream);
            }
            var worksheet = workbook.Worksheets[0];

            // コールバックに渡すためのIRangeオブジェクト
            // 都度生成すると、大きな帳票ではインスタンス生成コストが無視できない
            // 可能性があるため、インスタンスを使いまわす
            var range = new Range();
            // 利用している領域を走査して、単一項目を設定する
            var usedRange = worksheet.UsedRange;
            for (var i = 0; i < usedRange.Rows.Count; i++)
            {
                for (var j = 0; j < usedRange.Columns.Count; j++)
                {
                    var cell = usedRange[i, j];
                    if (cell.Value != null && _setters.ContainsKey(cell.Value))
                    {
                        range.DioDocsRange = cell;
                        _setters[cell.Value](range);
                    }
                }
            }

            var templateTable = worksheet.Tables[_tableName];

            // テーブルの行数を確認し、不足分を追加する
            if (templateTable.Rows.Count < rows.Count)
            {
                var addCount = rows.Count - templateTable.Rows.Count;
                for (var i = 0; i < addCount; i++)
                {
                    templateTable.Rows.Add(templateTable.Rows.Count - 1);
                }
            }

            // テーブルの1行目から項目の列番号を探索する
            var rowSetters = new List<(int index, Action<IRange, TReportRow> setter)>();
            var firstRow = templateTable.Rows[0];
            for (var i = 0; i < firstRow.Range.Columns.Count; i++)
            {
                var value = firstRow.Range[0, i].Value;
                if (value != null && _tableSetters.ContainsKey(value))
                {
                    rowSetters.Add((i, _tableSetters[value]));
                }
            }

            // テーブルに値を設定する
            for (var i = 0; i < rows.Count; i++)
            {
                var row = templateTable.Rows[i];
                foreach (var rowSetter in rowSetters)
                {
                    range.DioDocsRange = row.Range[rowSetter.index];
                    rowSetter.setter(range, rows[i]);
                }
            }

            workbook.Save(stream, (GrapeCity.Documents.Excel.SaveFileFormat)saveFileFormat);
        }
    }
}