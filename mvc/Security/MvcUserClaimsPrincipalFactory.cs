using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using mvc.Models;

namespace mvc.Security
{
    public class MvcUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<MvcUser>
    {
        public MvcUserClaimsPrincipalFactory(UserManager<MvcUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(MvcUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("locale", user.Locale));
            return identity;
        }
    }
}