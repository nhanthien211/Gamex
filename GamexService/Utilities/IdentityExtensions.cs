using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using GamexEntity.Constant;

namespace GamexService.Utilities
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
    }
}
