using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class PdfDocumentAttribute : Attribute
    {
        private readonly string documentId;


        public PdfDocumentAttribute(string documentId)
        {
            this.documentId = documentId;
        }

        public string DocumentId
        {
            get { return this.documentId; }
        }

    }

}