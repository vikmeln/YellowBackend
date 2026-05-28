using Amazon.S3;
using Amazon.S3.Model;
using ProductService.DTOs;
using ProductService.Services.Interface;

namespace ProductService.Services
{
    public class S3FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3;
        private readonly IConfiguration _configuration;

        private readonly string[] _allowedContentTypes =
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        public S3FileStorageService(
            IAmazonS3 s3,
            IConfiguration configuration
        )
        {
            _s3 = s3;
            _configuration = configuration;
        }

        public Task<UploadUrlResponseDto> CreateProductImageUploadUrlAsync(
            string fileName,
            string contentType
        )
        {
            if (!_allowedContentTypes.Contains(contentType))
            {
                throw new InvalidOperationException(
                    "Можно загружать только JPG, PNG или WEBP."
                );
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            var bucketName = _configuration["Storage:BucketName"];
            var publicBaseUrl = _configuration["Storage:PublicBaseUrl"];

            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new InvalidOperationException("Storage:BucketName не настроен.");
            }

            if (string.IsNullOrWhiteSpace(publicBaseUrl))
            {
                throw new InvalidOperationException("Storage:PublicBaseUrl не настроен.");
            }

            var key = $"products/{Guid.NewGuid():N}{extension}";

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ContentType = contentType
            };

            var uploadUrl = _s3.GetPreSignedURL(request);
            var imageUrl = $"{publicBaseUrl.TrimEnd('/')}/{key}";

            return Task.FromResult(new UploadUrlResponseDto
            {
                UploadUrl = uploadUrl,
                ImageUrl = imageUrl,
                Key = key
            });
        }
    }
}