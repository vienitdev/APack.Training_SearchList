using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.DirectoryServices;
using System.Security.Claims;
using System.DirectoryServices.AccountManagement;
using Archpack.Training.ArchUnits.Contracts.V1;

namespace Archpack.Training.ArchUnits.Authentications.V1
{
    /// <summary>
    /// Active Directory に対して LDAPサービスの機能を利用した認証を行います。
    /// </summary>
    [AuthenticationProviderAttribute(Name = "ad")]
    public class ADAuthenticationProvider : IAuthenticationProvider
    {
        private const string FilterString = "(SAMAccountName={0})";
        private const string DomainUserFormat = @"{0}\{1}";
        private const string DomainConfigurationKey = "authDomain";
        private const string CommonNameKey = "cn";


        /// <summary>
        /// 指定された<see cref="IOwinContext"/>を利用して、インスタンスを初期化します。
        /// </summary>
        public ADAuthenticationProvider()
        {
        }

        private string GetDomain()
        {
            string domain = System.Environment.UserDomainName;
            var config = ArchUnits.Configuration.V1.ServiceConfigurationLoader.Load();
            if (config.Raw.ContainsKey(DomainConfigurationKey))
            {
                domain = config.Raw[DomainConfigurationKey].ToString();
            }

            return domain;
        }

        /// <summary>
        /// 認証を行います
        /// </summary>
        /// <param name="request">認証の要求情報</param>
        /// <returns>認証が成功した場合の <see cref="Claim"/> を含む、認証結果</returns>
        public AuthenticationResult SignIn(AuthenticationRequest request)
        {

            Contract.NotNull(request, "authenticationrequest");
            var domain = GetDomain();
            AuthenticationResult result = ValidateUser(domain, request.Identifier, request.Password);

            if (result.IsAuthenticate)
            {
                result.Claims.Add(new Claim(ClaimTypes.Name, request.Identifier));
            }

            return result;
        }

        private AuthenticationResult ValidateUser(string domain, string username, string password)
        {
            AuthenticationResult result = new AuthenticationResult(AuthenticationStatus.Denied);

            string domainAndUsername = string.Format(DomainUserFormat, domain, username);
            var path = "LDAP://" + domain;
            DirectoryEntry entry = new DirectoryEntry(path, domainAndUsername, password);

            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);

                PrincipalContext context = new PrincipalContext(ContextType.Domain);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, domainAndUsername);
                if (user != null)
                {
                    if (user.IsAccountLockedOut())
                    {
                        result = new AuthenticationResult(AuthenticationStatus.AccountLockedOut);
                        return result;
                    }
                } 
                
                search.Filter = string.Format(FilterString, username);
                search.PropertiesToLoad.Add(CommonNameKey);
                SearchResult searchResult = search.FindOne();

                if (searchResult != null)
                {
                    result = new AuthenticationResult(AuthenticationStatus.Authenticated);
                }
            }
            catch
            {
                return result;
            }

            return result;
        }
    }
}