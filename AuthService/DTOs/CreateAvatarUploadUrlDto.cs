using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class CreateAvatarUploadUrlDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;
    }
}
