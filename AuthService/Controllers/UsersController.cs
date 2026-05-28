using AuthService.DTOs;
using AuthService.Models;
using AuthService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAvatarStorageService _avatarStorageService;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            IAvatarStorageService avatarStorageService
        )
        {
            _userManager = userManager;
            _avatarStorageService = avatarStorageService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.AvatarUrl
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.AvatarUrl
            });
        }

        [HttpPost("me/avatar/upload-url")]
        [Authorize]
        public async Task<IActionResult> CreateAvatarUploadUrl(
            CreateAvatarUploadUrlDto dto
        )
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized();
                }

                var result = await _avatarStorageService.CreateAvatarUploadUrlAsync(
                    userId,
                    dto.FileName,
                    dto.ContentType
                );

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("me/avatar")]
        [Authorize]
        public async Task<IActionResult> UpdateMyAvatar(UpdateAvatarDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.AvatarUrl = dto.AvatarUrl;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.AvatarUrl
            });
        }
    }
}
