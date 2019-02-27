using GamexApi.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GamexApi.Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IdentityContext", throwIfV1Schema: false)
        {
        }
    }
}