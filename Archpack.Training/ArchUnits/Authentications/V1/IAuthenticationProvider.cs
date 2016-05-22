using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.Security.Claims;

namespace Archpack.Training.ArchUnits.Authentications.V1
{
    /// <summary>
    /// 認証を行うプロバイダーのインターフェイスを定義します。
    /// </summary>
    public interface IAuthenticationProvider
    {

        AuthenticationResult SignIn(AuthenticationRequest request);
    }

}
