using Archpack.Training.ArchUnits.Authentications.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V1
{

    [AuthenticationProviderAttribute(Name = "arcsemp")]
    public class EmployeeAuthenticationProvider : IAuthenticationProvider
    {
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
            var identity = new GenericIdentity(request.Identifier);

            EmployeeInformation empInfo = new EmployeeInformation();
            var emp = empInfo.GetUserInfo(identity);

            if (emp != null)
            {
                return new AuthenticationResult(AuthenticationStatus.Authenticated);
            }

            return new AuthenticationResult(AuthenticationStatus.Denied);
        }
    }
}