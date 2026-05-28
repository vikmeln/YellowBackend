using AuthService.Models;

namespace AuthService.Services.Interface
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
    }
}
