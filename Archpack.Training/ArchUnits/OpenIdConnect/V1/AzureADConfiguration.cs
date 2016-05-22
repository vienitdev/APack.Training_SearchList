using Archpack.Training.ArchUnits.Configuration.V1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.OpenIdConnect.V1
{
    public class AzureADConfiguration
    {

        private const string RootKey = "azureAdSettings";

        public AzureADConfiguration()
        {
            var config = ServiceConfigurationLoader.Load();
            if (!config.Raw.ContainsKey(RootKey))
            {
                throw new InvalidOperationException("構成情報が設定されていません。");
            }
            var values = config.Raw[RootKey].ToObject<Dictionary<string, string>>();

            ClientId = values["clientId"];
            AADInstance = values["aadInstance"];
            Tenant = values["tenant"];
            PostLogoutRedirectUri = values["postLogoutRedirectUri"];
            AppKey = values["appKey"];
            Authority = String.Format(CultureInfo.InvariantCulture, AADInstance, Tenant);
            ResourceId = values["resourceId"];
            GraphResourceId = values["graphResourceId"];

            //ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
            //AADInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
            //Tenant = ConfigurationManager.AppSettings["ida:Tenant"];
            //PostLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
            //AppKey = ConfigurationManager.AppSettings["ida:AppKey"];
            //Authority = String.Format(CultureInfo.InvariantCulture, AADInstance, Tenant);
            //ResourceId = ConfigurationManager.AppSettings["ida:ResourceId"];
            //GraphResourceId = ConfigurationManager.AppSettings["ida:GraphResourceId"];
        }

        public const string ObjectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public string ClientId { get; private set; }
        public string ResourceId { get; private set; }
        public string AADInstance { get; private set; }
        public string Tenant { get; private set; }
        public string PostLogoutRedirectUri { get; private set; }
        public string AppKey { get; private set; }
        public string Authority { get; private set; }
        public string GraphResourceId { get; private set; }


    }
}