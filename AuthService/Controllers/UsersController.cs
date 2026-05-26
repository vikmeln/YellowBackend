using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users
                .Select(u => new
                {
                    u.UserName
                })
                .ToList();
            return Ok(users);
        }
    }
}
