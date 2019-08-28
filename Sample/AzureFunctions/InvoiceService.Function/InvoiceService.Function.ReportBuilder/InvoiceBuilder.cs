using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GrapeCity.Documents.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveFileFormat = InvoiceService.Function.ReportBuilder.SaveFileFormat;

namespace InvoiceService.Function.ReportBuilder
{
    public class InvoiceBuilder
    {
        static InvoiceBuilder() => Workbook.SetLicenseKey(Secrets.DioDocsKey);

        public Action Init { get; set; }
        public Func<Stream> GetRequestStream { get; set; }

        public Func<string, Stream> GetTemplateStream { get; set; }

        public Action<byte[]> Output { get; set; }

        public void Build()
        {
            Execute("Total", () =>
            {
                Execute("Init", () => Init());

                string template = null;
                JToken data = null;
                Execute("LoadData", () =>
                {
                    using (var jsonTextReader = new JsonTextReader(new StreamReader(GetRequestStream())))
                    {
                        var request = JToken.Load(jsonTextReader);
                        template = request["Template"].Value<string>();
                        data = request["Data"];
                    }
                });

                JsonReportBuilder reportBuilder = null;
                Execute("LoadTemplate", () =>
                {
                    using (var stream = GetTemplateStream(template))
                    {
                        reportBuilder = new JsonReportBuilder(stream, this);
                    }
                });

                byte[] pdf = null;
                Execute("Build", () =>
                {
                    using (var output = new MemoryStream())
                    {
                        reportBuilder.Build(data, output, SaveFileFormat.Pdf);
                        pdf = output.ToArray();
                    }
                });

                Execute("Upload", () =>
                {
                    Output(pdf);
                });
            });
        }

        public List<Tuple<string, TimeSpan>> MeasurementResults { get; } = new List<Tuple<string, TimeSpan>>();

        public override string ToString()
        {
            return string.Join(", ", MeasurementResults.Select(x => $"{x.Item1}:{x.Item2}"));
        }


        public class JsonReportBuilder : IJsonReportBuilder
        {
            private readonly InvoiceBuilder _invoiceBuilder;
            private readonly byte[] _template;
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

            public JsonReportBuilder(Stream template, InvoiceBuilder invoiceBuilder)
            {
                _invoiceBuilder = invoiceBuilder;
                _template = new byte[template.Length];
                template.Read(_template, 0, _template.Length);
            }

            public void Build(TextReader reader, Stream output, SaveFileFormat saveFileFormat)
            {
                using (var jsonTextReader = new JsonTextReader(reader))
                {
                    Build(JToken.ReadFrom(jsonTextReader), output, saveFileFormat);
                }
            }

            public IWorkbook OpenWorkbook()
            {
                IWorkbook workbook = null;
                _invoiceBuilder.Execute("OpenWorkbook", () =>
                {
                    using (var memoryStream = new MemoryStream(_template))
                    {
                        workbook = new Workbook();
                        workbook.Open(memoryStream);
                    }
                });
                return workbook;
            }

            public void ParseSettings(IWorkbook workbook)
            {
                _invoiceBuilder.Execute("ParseSettings", () =>
                {
                    var worksheet = workbook.Worksheets["DioDocs.FastReportBuilder"];
                    ParseSettings(worksheet);
                    worksheet.Delete();
                });
            }

            public void SetSingleCells(IWorkbook workbook, JToken input)
            {
                _invoiceBuilder.Execute("SetSingleCells", () =>
                {
                    var worksheet = workbook.Worksheets[0];
                    foreach (var rangeAccessor in _accessors)
                    {
                        rangeAccessor.Set(worksheet, input);
                    }
                });
            }

            public void SetTableCells(IWorkbook workbook, JToken input)
            {
                _invoiceBuilder.Execute("SetTableCells", () =>
                {
                    var worksheet = workbook.Worksheets[0];
                    if (_tableName != null)
                    {
                        var table = worksheet.Tables[_tableName];
                        var rows = input[_tableName];

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
                });
            }

            public void SavePdf(IWorkbook workbook, Stream output, SaveFileFormat saveFileFormat)
            {
                _invoiceBuilder.Execute("SavePdf", () => workbook.Save(output, (GrapeCity.Documents.Excel.SaveFileFormat)saveFileFormat));
            }

            public void Build(JToken input, Stream output, SaveFileFormat saveFileFormat)
            {
                var workbook = OpenWorkbook();
                ParseSettings(workbook);
                SetSingleCells(workbook, input);
                SetTableCells(workbook, input);
                SavePdf(workbook, output, saveFileFormat);
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
            }
        }


        private void Execute(string section, Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                action();
            }
            finally
            {
                stopwatch.Stop();
                MeasurementResults.Add((new Tuple<string, TimeSpan>(section, stopwatch.Elapsed)));
            }
        }
    }
}