using Identity.Model;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}