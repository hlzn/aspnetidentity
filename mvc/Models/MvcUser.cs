

using Microsoft.AspNetCore.Identity;

namespace mvc.Models
{
    public class MvcUser : IdentityUser
    {
        public string Locale { get; set; } = "en-US";
        public string OrgId { get; set; }
    }

    public class Organization
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}