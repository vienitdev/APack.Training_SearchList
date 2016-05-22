using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1
{
    /// <summary>
    /// IPdfService
    /// </summary>
    public interface IPdfService
    {
        string Url { get; set; }

        /// <summary>
        /// CreatePDF
        /// </summary>
        /// <param name="soapParameters"></param>
        byte[] createPDF(Parameter[] soapParameters);

        /// <summary>
        /// CreatePDFAsync
        /// </summary>
        /// <param name="soapParameters"></param>
        void createPDFAsync(Parameter[] soapParameters);

        /// <summary>
        /// CreatePDFAsync
        /// </summary>
        /// <param name="soapParameters"></param>
        /// <param name="userState"></param>
        void createPDFAsync(Parameter[] soapParameters, object userState);

        /// <summary>
        /// CancelAsync
        /// </summary>
        /// <param name="userState"></param>
        void CancelAsync(object userState);
    }
}