using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Archpack.Training.ServiceUnits.Trainings.V1
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected readonly string lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        protected object namespaces;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                //if (isLogout.Value.ToLower() == true.ToString().ToLower())
                //{
                //    SignOut();
                //}
            }
            SetUserInfo();
            GetApplicationVersion();
        }

        private void SetUserInfo()
        {
            //this.userId.InnerText = Page.User.Identity.Name;
        }

        private void SignOut()
        {
            if (FormsAuthentication.IsEnabled)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.RedirectToLoginPage();
            }
        }

        private void GetApplicationVersion()
        {
            //const string NamespaceLiteral = "Namespace";

            //JObject apps = new JObject();

            //var test = Settings.Default.Properties.Cast<SettingsProperty>();
            //var props = Settings.Default.Properties.Cast<SettingsProperty>()
            //            .Where(x => x.Name.Contains(NamespaceLiteral)).ToList();

            //foreach (var prop in props)
            //{
            //    apps.Add(prop.Name.Replace(NamespaceLiteral, ""), prop.DefaultValue.ToString());
            //}
            //this.namespaces = apps;
        }
    }
}