using CategoryService.DTOs;
using CategoryService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CategoryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            var category = await _service.UpdateAsync(id, dto);
            return Ok(category);
        }
    }
}
