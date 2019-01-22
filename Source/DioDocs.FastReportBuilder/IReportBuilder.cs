using System;
using System.Collections.Generic;
using System.IO;

namespace DioDocs.FastReportBuilder
{
    public interface IReportBuilder<TReportRow>
    {
        /// <summary>
        /// 単項目をアプリケーション側に設定させるため、設定対象のセル（IRange）を引数に
        /// コールバックさせるためのActionを登録する。
        /// </summary>
        IReportBuilder<TReportRow> AddSetter(object key, Action<IRange> setter);

        /// <summary>
        /// 表項目をアプリケーション側に設定させるため、設定対象のセル（IRange）を引数に
        /// コールバックさせるためのActionを登録する。
        /// </summary>
        IReportBuilder<TReportRow> AddTableSetter(string key, Action<IRange, TReportRow> setter);

        /// <summary>
        /// 表の行オブジェクトを引数に指定して帳票を生成する
        /// </summary>
        void Build(IList<TReportRow> rows, Stream stream, SaveFileFormat saveFileFormat);
    }
}