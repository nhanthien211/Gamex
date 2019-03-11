using GamexEntity.Constant;
using Microsoft.Owin.Security.Facebook;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GamexApi.Providers
{
    public class FacebookAuthProvider : FacebookAuthenticationProvider {
        public override Task Authenticated(FacebookAuthenticatedContext context) {
            var firstName = context.User.GetValue("first_name").ToString();
            var lastname = context.User.GetValue("last_name").ToString();
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            context.Identity.AddClaim(new Claim(CustomClaimTypes.Email, context.Email, "XmlSchemaString", "Facebook"));
            context.Identity.AddClaim(new Claim(CustomClaimTypes.FirstName, firstName, "XmlSchemaString", "Facebook"));
            context.Identity.AddClaim(new Claim(CustomClaimTypes.LastName, lastname, "XmlSchemaString", "Facebook"));
            return Task.FromResult<object>(null);
        }

    }
}