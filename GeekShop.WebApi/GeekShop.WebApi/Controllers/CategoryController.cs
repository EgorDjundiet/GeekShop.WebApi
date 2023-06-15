using GeekShop.Domain.ViewModels;
using GeekShop.Domain;
using Microsoft.AspNetCore.Mvc;
using GeekShop.Services.Contracts;

namespace GeekShop.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("GetCategoryHierarchy")]
        public async Task<CategoryTree> GetCategoryHierarchy()
        {
            return await _categoryService.GetCategoryHierarchy();
        }
        [HttpGet("GetBreadcrumbs")]
        public async Task<CategoryTree> GetBreadcrumbs(int id)
        {
            return await _categoryService.GetBreadcrumbs(id);
        }
        [HttpGet("GetAll")]
        public async Task<IEnumerable<CategoryWithParent>> GetAll()
        {
            return await _categoryService.GetAll();
        }
        [HttpGet("Get")]
        public async Task<CategoryWithParent> Get(int id)
        {
            return await _categoryService.Get(id);
        }
        [HttpPost("Add")]
        public async Task<CategoryWithParent> Add([FromBody] SubmitCategoryWithParentIn category)
        {
            return await _categoryService.Add(category);
        }
        [HttpPut("Update")]
        public async Task Update(int id, [FromBody] SubmitCategoryWithParentIn category)
        {
            await _categoryService.Update(id, category);
        }
        [HttpDelete("Delete")]
        public async Task Delete(int id)
        {
            await _categoryService.Delete(id);
        }
    }
}
