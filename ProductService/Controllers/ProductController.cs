using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.Services.Interface;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IFileStorageService _fileStorageService;

        public ProductsController(
            IProductService service,
            IFileStorageService fileStorageService
        )
        {
            _service = service;
            _fileStorageService = fileStorageService;
        }

        [HttpPost("upload-url")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUploadUrl(CreateUploadUrlDto dto)
        {
            var result = await _fileStorageService.CreateProductImageUploadUrlAsync(
                dto.FileName,
                dto.ContentType
            );

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetActive()
        {
            var products = await _service.GetActiveAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var product = await _service.CreateAsync(dto);
            return Ok(product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var product = await _service.UpdateAsync(id, dto);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(int id)
        {
            await _service.ActivateAsync(id);
            return NoContent();
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAdmin(int id)
        {
            var product = await _service.GetByIdForAdminAsync(id);
            return Ok(product);
        }

        [HttpPost("decrease-stock")]
        [Authorize]
        public async Task<IActionResult> DecreaseStock(StockDto dto)
        {
            await _service.DecreaseStockAsync(dto.ProductId, dto.Quantity);
            return Ok();
        }

        [HttpPost("restore-stock")]
        [Authorize]
        public async Task<IActionResult> RestoreStock(StockDto dto)
        {
            await _service.RestoreStockAsync(dto.ProductId, dto.Quantity);
            return Ok();
        }
    }
}
