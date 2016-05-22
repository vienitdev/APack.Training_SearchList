using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.Security.Claims;
using Archpack.Training.ArchUnits.Contracts.V1;

namespace Archpack.Training.ArchUnits.Authentications.V1
{
    /// <summary>
    /// ASP.NET Membership を利用した認証を行います。
    /// </summary>
    [AuthenticationProviderAttribute(Name = "db")]
    public class DatabaseAuthenticationProvider: IAuthenticationProvider
    {

        /// <summary>
        /// 指定された<see cref="IOwinContext"/>を利用して、インスタンスを初期化します。
        /// </summary>
        public DatabaseAuthenticationProvider()
        {
        }

        /// <summary>
        /// 認証を行います
        /// </summary>
        /// <param name="request">認証の要求情報</param>
        /// <returns>認証が成功した場合の <see cref="Claim"/> を含む、認証結果</returns>
        public AuthenticationResult SignIn(AuthenticationRequest request)
        {
            Contract.NotNull(request, "request");
            AuthenticationResult result = new AuthenticationResult(AuthenticationStatus.Denied);

            if (System.Web.Security.Membership.ValidateUser(request.Identifier, request.Password))
            {
                result = new V1.AuthenticationResult(AuthenticationStatus.Authenticated);
                result.Claims.Add(new Claim(ClaimTypes.Name, request.Identifier));
                return result;
            }
            else
            {
                return result;
            }
        }

        
    }

    
}