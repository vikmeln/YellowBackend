using ProductService.DTOs;

namespace ProductService.Services.Interface
{
    public interface IFileStorageService
    {
        Task<UploadUrlResponseDto> CreateProductImageUploadUrlAsync(
            string fileName,
            string contentType
        );
    }
}
