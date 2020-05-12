using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace mvc.Security
{
    public class DoesNotContainPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser: class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var username = await manager.GetUserNameAsync(user);
            if (username == password)
            {
                return IdentityResult.Failed(new IdentityError{Description="Password cannot be username"});
            }
            if (password.Contains("password"))
            {
                return IdentityResult.Failed(new IdentityError{Description="Password cannot contain word password"});
            }
            return IdentityResult.Success;
        }
    }
}