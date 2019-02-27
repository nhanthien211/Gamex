using GamexEntity.Constant;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Owin.Security;

namespace GamexWeb.Utilities
{
    public static class IdentityExtensions
    {
        internal static string FirstOrNull(this ClaimsIdentity identity, string claimType)
        {
            var result = identity.FindFirst(claimType);
            return result == null ? null : result.Value;
        }

        public static string GetFullName(this IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            return (identity as ClaimsIdentity).FirstOrNull(CustomClaimTypes.UserFullName);
        }

        public static string GetEmail(this IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }
            return (identity as ClaimsIdentity).FirstOrNull(CustomClaimTypes.Email);
        }

        public static void AddUpdateClaim(this IPrincipal currentPrincipal, string key, string value, IAuthenticationManager authenticationManager)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
        }
    }
}
