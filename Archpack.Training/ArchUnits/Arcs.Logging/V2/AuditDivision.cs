using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Logging.V2
{
    /// <summary>
    /// 監査ログの区分を表します
    /// </summary>
    public enum AuditDivision: byte
    {
        /// <summary>
        /// 一覧参照
        /// </summary>
        ListReference = 1,
        /// <summary>
        /// 詳細参照
        /// </summary>
        DetailReference = 2,
        /// <summary>
        /// CSV出力
        /// </summary>
        OutputCsv = 3,
        /// <summary>
        /// 帳票印刷
        /// </summary>
        ReportPrint = 4,
        /// <summary>
        /// ファイルダウンロード
        /// </summary>
        FileDownload = 5
    }
}
