using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public class CreateUploadUrlDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;
    }
}
