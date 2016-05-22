using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.Security.Claims;
using Archpack.Training.ArchUnits.OpenIdConnect.V1;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Archpack.Training.ArchUnits.Contracts.V1;

namespace Archpack.Training.ArchUnits.Authentications.V1
{
    /// <summary>
    /// Azure Active Directory に対して認証を行います。
    /// </summary>
    [AuthenticationProviderAttribute(Name = "azuread")]
    public class AzureAuthenticationProvider : IAuthenticationProvider
    {        
        private AzureADConfiguration aadConfig = null;

        /// <summary>
        /// 指定された<see cref="IOwinContext"/>を利用して、インスタンスを初期化します。
        /// </summary>
        public AzureAuthenticationProvider()
        {
            aadConfig = new AzureADConfiguration();
        }


        /// <summary>
        /// 認証を行います
        /// </summary>
        /// <param name="request">認証の要求情報</param>
        /// <returns>認証が成功した場合の <see cref="Claim"/> を含む、認証結果</returns>
        public AuthenticationResult SignIn(AuthenticationRequest request)
        {
            Contract.NotNull(request, "request");
            AuthenticationResult result = ValidateUser(request);

            if (result.IsAuthenticate)
            {
                result.Claims.Add(new Claim(ClaimTypes.Name, request.Identifier));
            }

            return result;
        }

        private AuthenticationResult ValidateUser(AuthenticationRequest request)
        {
            AuthenticationResult result = new AuthenticationResult(AuthenticationStatus.Denied);

            try
            {
                var openIdResult = ArchUnits.OpenIdConnect.V1.OpenIdConnectAuthenticationExtensions.ValidateUser(request.Identifier, request.Password);

                return new AuthenticationResult(AuthenticationStatus.Authenticated);;
            }
            catch (AdalServiceException ex)
            {
                if (ex.ServiceErrorCodes.Contains("AADSTS50126") || ex.ServiceErrorCodes.Contains("50126"))
                {
                    return result;
                }

                throw;
            }
            catch (AdalException)
            {
                return result;
            }

        }
    }
}