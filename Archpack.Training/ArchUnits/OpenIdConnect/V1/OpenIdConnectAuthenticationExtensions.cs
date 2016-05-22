using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Archpack.Training.ArchUnits.OpenIdConnect.V1
{
    public static class OpenIdConnectAuthenticationExtensions
    {
        private static readonly AzureADConfiguration aadConfig = new AzureADConfiguration();

        public static void UseOpenIdConnectAuthentication(this IAppBuilder app)
        {
            //認証タイプとして既定のサインインをCookie認証にします。
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = aadConfig.ClientId,
                Authority = aadConfig.Authority,
                PostLogoutRedirectUri = aadConfig.PostLogoutRedirectUri,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("~/Shared/Error?message=" + context.Exception.Message);
                        return Task.FromResult(0);
                    },
                    AuthorizationCodeReceived = (context) =>
                    {
                        var code = context.Code;
                        ClientCredential credential = new ClientCredential(aadConfig.ClientId, aadConfig.AppKey);
                        AuthenticationContext authContext = new AuthenticationContext(aadConfig.Authority);
                        AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, aadConfig.GraphResourceId);

                        return Task.FromResult(0);
                    }
                }
            });

            app.UseCors(CorsOptions.AllowAll);

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            {
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = aadConfig.ClientId

                },
                Tenant = aadConfig.Tenant
            });

        }

        public static AuthenticationResult GetAuthenticationResult(string resoureceId)
        {
            string userObjectID = ClaimsPrincipal.Current.FindFirst(AzureADConfiguration.ObjectIdClaimType).Value;
            AuthenticationContext authContext = new AuthenticationContext(aadConfig.Authority);
            ClientCredential credential = new ClientCredential(aadConfig.ClientId, aadConfig.AppKey);

            var result = authContext.AcquireTokenSilent(resoureceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));

            return result;
        }

        public static AuthenticationResult ValidateUser(string userid, string password)
        {
            AuthenticationContext authContext = new AuthenticationContext(aadConfig.Authority, false);
            authContext.TokenCache.Clear();

            UserCredential credential = new UserCredential(userid, password);
            var result = authContext.AcquireToken(aadConfig.GraphResourceId, aadConfig.ClientId, credential);
            return result;
        }
    }
}