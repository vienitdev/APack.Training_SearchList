using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.Validations.V1;
using Newtonsoft.Json.Linq;
using Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1;
using System.Security.Principal;

namespace Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1
{
    public interface IPdfDocumentAction
    {
        
        ValidationResult ValidateParamters(JObject data);

        IEnumerable<DocumentData> ConvertToDocumentData(JObject data);

        IIdentity Identity { get; set; }
    }
}