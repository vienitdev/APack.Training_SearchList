using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1
{
    /// <summary>
    /// PDFDocument例外
    /// </summary>
    public class PDFDocumentException : Exception
    {
        /// <summary>
        /// PDFDocument例外のコンストラクタ
        /// </summary>
        public PDFDocumentException() : base() { }

        /// <summary>
        /// PDFDocument例外のコンストラクタ
        /// </summary>
        /// <param name="message"></param>
        public PDFDocumentException(string message) : base(message) { }

        /// <summary>
        /// PDFDocument例外のコンストラクタ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PDFDocumentException(string message, Exception innerException) : base(message, innerException) { }
    }
}