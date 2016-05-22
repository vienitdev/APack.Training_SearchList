
using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Archpack.Training.ServiceUnits.Shared.V1.Anonymous.Pages
{
    public partial class _500 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ErrorMessage = string.Empty;
            this.ErrorStackTrace = string.Empty;

            Exception exception = Server.GetLastError();
            if (exception == null)
            {
                exception = Context.Items["Error"] as Exception;
            }

            if (exception != null)
            {
                this.ErrorMessage = exception.Message;
                this.ErrorStackTrace = exception.StackTrace;
            }
        }

        public string RedirectUrl
        {
            get
            {
                object url = Context.Items["RedirectUrl"];

                if (url != null)
                {
                    return url.ToString();
                }

                return VirtualPathUtility.GetDirectory("~/");
            }
        }

        public string ErrorMessage { get; private set; }
        public string ErrorStackTrace { get; private set; }
    }
}