using Microsoft.AspNetCore.Identity;

namespace AuthService.Services.Interface
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user, IList<string> roles);
    }
}
