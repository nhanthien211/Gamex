using Microsoft.AspNet.Identity.EntityFramework;

namespace GamexWeb.Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IdentityContext", throwIfV1Schema: false)
        {
        }
    }
}