using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Archpack.Training.ServiceUnits.Shared.V1.Users.Pages
{
    public partial class DownloadError : System.Web.UI.Page
    {
        public string Message = "";
        public string Detail = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Message = Request.QueryString["Message"];
                Detail = Request.QueryString["Detail"];
            }
        }
    }
}