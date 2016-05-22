using Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.ArchUnits.Validations.V1;
using Archpack.Training.ArchUnits.WebApiExtensions.V1;
using Archpack.Training.ArchUnits.WebApiModels.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace Archpack.Training.ServiceUnits.Shared.V1.Users.Api
{

    public class PdfDocumentController : ApiController
    {
        private static readonly Dictionary<string, IPdfDocumentAction> DocumentActions;

        static PdfDocumentController()
        {
            DocumentActions = new Dictionary<string, IPdfDocumentAction>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    var actionTypes = (from t in types
                                       where typeof(IPdfDocumentAction).IsAssignableFrom(t)
                                       select t).ToArray();
                    foreach (var action in actionTypes)
                    {
                        try
                        {
                            var attribute = action.GetCustomAttribute<PdfDocumentAttribute>();
                            DocumentActions.Add(attribute.DocumentId, Activator.CreateInstance(action) as IPdfDocumentAction);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        private ServiceUnitContext serviceUnitContext;

        public PdfDocumentController(ServiceUnitContext serviceUnitContext)
        {
            this.serviceUnitContext = serviceUnitContext;
        }

        [HttpPost]
        public HttpResponseMessage Download([FromUri]string documentId, [FromBody]JObject data)
        {
            try
            {
                Contract.NotEmpty(documentId, "帳票ID");
                Contract.NotNull(data, "データ");

                if (data["data"] != null)
                {
                    data = JObject.Parse(data["data"].ToString());
                }

                IPdfDocumentAction action = FindPdfDocumentAction(documentId);

                if (action == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.Messages.NotFoundData);
                }

                action.Identity = this.RequestContext.Principal.Identity;

                ValidationResult validationResult = action.ValidateParamters(data);

                if (!validationResult.IsValid)
                {
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, validationResult.CreateWebApiErrorResponse());
                }

                IEnumerable<DocumentData> documentData = action.ConvertToDocumentData(data);
                PdfDocument pdfDoc = new PdfDocument();
                byte[] result = pdfDoc.CreatePdf(documentData.ToArray());


                MemoryStream stream = new MemoryStream(result);
                return this.Request.CreateFileResponse(HttpStatusCode.OK, stream, documentId + ".pdf", "application/pdf");
            }
            catch(Exception ex)
            {
                WriteErrorLog(ex);
                return this.Request.CreateFileDownloadErrorResponse(HttpStatusCode.Redirect, Resources.Messages.PdfServiceError, string.Empty);
            }
        }

        private IPdfDocumentAction FindPdfDocumentAction(string documentId)
        {
            IPdfDocumentAction action = DocumentActions[documentId] as IPdfDocumentAction;

            if (action == null)
            {
                return null;
            }

            return action;
        }

        private void WriteErrorLog(Exception exception) 
        {
            LogData logData = new LogData()
            {
                Exception = exception,
                Uri = serviceUnitContext.Request.Path,
                User = serviceUnitContext.User == null ? null : serviceUnitContext.User.Identity.Name,
                LogName = "error",
                LogId = serviceUnitContext.LogContext.Id
            };
            serviceUnitContext.LogContext.Logger.Error(logData);
        }
    }

}
