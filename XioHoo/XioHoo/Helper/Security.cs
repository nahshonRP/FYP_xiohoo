using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourseMangement.Helper
{
    public static class Security
    {
        public static ClaimsPrincipal SetPrincipal()
        {
            return
                           new ClaimsPrincipal(
                              new ClaimsIdentity(
                                 new Claim[] {
                                            new Claim(ClaimTypes.NameIdentifier, "userid"),
                                            new Claim(ClaimTypes.Name,"name"),
                                            new Claim(ClaimTypes.Role, "RoleName"),
                                            new Claim(ClaimTypes.Sid, "username"),
                                 }, "Basic"
                              )
                           );
        }
    }


}
