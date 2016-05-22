using Archpack.Training.ArchUnits.Authentications.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V2
{
    [AuthenticationProviderAttribute(Name = "actosdb")]
    public sealed class ActosDbAuthenticationProvider : IAuthenticationProvider
    {
        public AuthenticationResult SignIn(AuthenticationRequest request)
        {
            var manager = new ActosDbAuthManager();
            var user = manager.FindUser(request.Identifier);
            if (user == null)
            {
                return new AuthenticationResult(AuthenticationStatus.Denied);
            }
            var passwordHash = ActosDbAuthManager.GeneratePasswordHash(request.Password);
            if(user.PassKey != passwordHash)
            {
                return new AuthenticationResult(AuthenticationStatus.Denied);
            }

            var result = new AuthenticationResult(AuthenticationStatus.Authenticated);
            result.Claims.Add(new Claim(ClaimTypes.Name, request.Identifier));
            return result;
        }
    }
}