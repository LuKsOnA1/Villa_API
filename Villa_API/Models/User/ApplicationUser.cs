using Microsoft.AspNetCore.Identity;

namespace Villa_API.Models.User
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
