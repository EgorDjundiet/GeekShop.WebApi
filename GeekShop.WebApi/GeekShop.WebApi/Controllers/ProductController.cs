using GeekShop.Domain;
using GeekShop.Domain.ViewModels;
using GeekShop.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeekShop.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("SeedData")]
        public async Task SeedData()
        {
            await _productService.SeedData();
        }
        [HttpGet("GetAll")]
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productService.GetAll();
        }
        
        [HttpGet("Get")]
        public async Task<Product> Get(int id)
        {
            return await _productService.Get(id);
        }

        [HttpPost("GetByIds")]
        public async Task<IEnumerable<Product>> GetByIds([FromBody]IEnumerable<int> ids)
        {
            return await _productService.GetByIds(ids);           
        }

        [HttpPost("Add")]
        public async Task<Product> Add([FromBody]SubmitProductIn productIn)
        {
            return await _productService.Add(productIn);              
        }       
        [HttpPut("Update")]
        public async Task Update(int id, [FromBody]SubmitProductIn productIn)
        {
            await _productService.Update(id,productIn);
        }
        [HttpDelete("Delete")]
        public async Task Delete(int id)
        {
            await _productService.Delete(id);
        }
    }
}
