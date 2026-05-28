using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class UpdateAvatarDto
    {
        [Required]
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
