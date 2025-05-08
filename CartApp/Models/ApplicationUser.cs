using Microsoft.AspNetCore.Identity;

namespace CartApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public List<Order> Orders { get; set; }
    }
}