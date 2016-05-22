using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1
{
    /// <summary>
    /// 帳票ID ごとのデータを格納します
    /// </summary>
    public class DocumentData
    {

        /// <summary>
        /// 帳票ID を指定して帳票データのインスタンスを初期化します。
        /// </summary>
        /// <param name="documentId">帳票ID</param>
        public DocumentData(string documentId) 
        {
            Contract.NotEmpty(documentId, "documentId");

            this.DocumentId = documentId;
            this.DataGroups = new List<DataGroup>();
        }

        /// <summary>
        /// 帳票ID を取得します。
        /// </summary>
        public string DocumentId { get; private set; }


        /// <summary>
        /// グループ設定のリストを取得します。
        /// </summary>
        public List<DataGroup> DataGroups { get; private set; }

    }
}