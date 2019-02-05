using GamexWeb.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GamexWeb.Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("GamexContext", throwIfV1Schema: false)
        {
        }
    }
}