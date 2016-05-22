using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiModels.V1
{
    /// <summary>
    /// ヘッダー明細一括更新リクエスト
    /// </summary>
    public class HeaderDetailChangeRequest<THeader, TDetail>
        where THeader : class
        where TDetail : class
    {
        /// <summary>
        /// デフォルト コンストラクタです。
        /// </summary>
        public HeaderDetailChangeRequest()
        {
            this.Header = new ChangeSet<THeader>();
            this.Detail = new ChangeSet<TDetail>();
        }

        /// <summary>
        /// ヘッダーの変更セットを取得または設定します。
        /// </summary>
        public ChangeSet<THeader> Header { get; set; }

        /// <summary>
        /// 明細の変更セットを取得または設定します。
        /// </summary>
        public ChangeSet<TDetail> Detail { get; set; }
    }
}