using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Contracts.V1;

namespace Archpack.Training.ArchUnits.Authentications.V1
{
    /// <summary>
    /// 構成情報に指定されている認証プロバイダーを利用して認証を実行します。
    /// </summary>
    public class Authentication
    {

        private const string ApplicationCookie = "ApplicationCookie";

        private IOwinContext owinContext = null;
        
        /// <summary>
        /// Authenticationのコンストラクタ
        /// </summary>
        /// <param name="owinContext">OWINコンテキスト</param>
        public Authentication(IOwinContext owinContext)
        {
            Contract.NotNull(owinContext, "owinContext");
            this.owinContext = owinContext;
        }

        /// <summary>
        /// 構成情報に指定された認証を順次実行し、成功した時点でその結果を返します。
        /// アカウントロックアウトの場合にも後続の認証プロバイダの処理を行わずエラーを返却します。
        /// </summary>
        /// <param name="request">認証の要求情報</param>
        /// <returns>認証が成功した場合の <see cref="Claim"/> を含む、認証結果</returns>
        public AuthenticationResult SignIn(AuthenticationRequest request)
        {
            var providers = GetProvidersFromConfigurations();
            var result = new AuthenticationResult(AuthenticationStatus.Denied);
            foreach (var provider in providers)
            {
                result = provider.SignIn(request);
                if (result.IsAuthenticate && result.Claims.Count > 0)
                {
                    ClaimsIdentity identity = new ClaimsIdentity(result.Claims, ApplicationCookie);
                    owinContext.Authentication.SignIn(new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = false,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7),
                    }, identity);

                    return result;
                }
                else if (result.Status == AuthenticationStatus.AccountLockedOut)
                {
                    return result;
                }
            }

            return result;
        }

        private IEnumerable<IAuthenticationProvider> GetProvidersFromConfigurations()
        {
            //Rootからロードする
            var configuration = ServiceConfigurationLoader.Load();
            if (!configuration.Raw.ContainsKey("authProviders"))
            {
                throw new InvalidOperationException();
            }
            var providerNames = configuration.Raw["authProviders"].ToObject<List<string>>();
            List<IAuthenticationProvider> providers = new List<IAuthenticationProvider>();

            foreach (string providerName in providerNames)
            {
                var types = Assembly.GetExecutingAssembly().GetTypes();
                var providerTypes = from t in types
                                    where typeof(IAuthenticationProvider).IsAssignableFrom(t) && 
                                          t.GetCustomAttributes<AuthenticationProviderAttribute>().Any(x => x.Name == providerName)
                                    select t;

                foreach (var type in providerTypes)
                {
                    providers.Add(Activator.CreateInstance(type) as IAuthenticationProvider);
                }
            }

            return providers;            
        }

        private const string DefaultLoginPage = "~/Shared/V1/Anonymous/page/Login";

        private static string GetDefaultLoginPage(ServiceConfiguration configuration)
        {
            var loginPageValue = configuration.Raw["loginPage"];
            string loginPage = DefaultLoginPage;

            if (loginPageValue != null)
            {
                loginPage = loginPageValue.ToString();
            }

            if (loginPage.StartsWith("~"))
            {
                loginPage = loginPage.Substring(1);
            }

            return loginPage;

        }
        /// <summary>
        /// 認証処理の初期化を実行します。
        /// </summary>
        /// <param name="app">アプリケーション定義</param>
        public static void ConfigureAuth(IAppBuilder app)
        {
            var configuration = ServiceConfigurationLoader.Load();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = ApplicationCookie,
                LoginPath = new PathString(GetDefaultLoginPage(configuration)),
                Provider = new CookieAuthenticationProvider()
            });
        }
    }

}