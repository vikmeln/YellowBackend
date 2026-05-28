using AuthService.DTOs;

namespace AuthService.Services.Interface
{
    public interface IAvatarStorageService
    {
        Task<AvatarUploadUrlResponseDto> CreateAvatarUploadUrlAsync(
            string userId,
            string fileName,
            string contentType
        );
    }
}
