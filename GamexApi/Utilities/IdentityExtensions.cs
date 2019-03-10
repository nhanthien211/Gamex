using System.Security.Claims;
using System.Security.Principal;
using GamexEntity.Constant;
using Microsoft.Owin.Security;

namespace GamexApi.Utilities
{
    public static class IdentityExtensions
    {
        internal static string FirstOrNull(this ClaimsIdentity identity, string claimType)
        {
            var result = identity.FindFirst(claimType);
            return result == null ? null : result.Value;
        }

        public static string GetEmail(this IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }
            return (identity as ClaimsIdentity).FirstOrNull(CustomClaimTypes.Email);
        }

        public static string GetFirstName(this IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }
            return (identity as ClaimsIdentity).FirstOrNull(CustomClaimTypes.FirstName);
        }

        public static string GetLastName(this IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }
            return (identity as ClaimsIdentity).FirstOrNull(CustomClaimTypes.LastName);
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
