using GeekShop.Domain;
using GeekShop.Repositories;
using GeekShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeekShop.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productService.GetAll();
        }

        [HttpGet("Get")]
        public async Task<Product?> Get(int id)
        {
            return await _productService.Get(id);
        }

        [HttpPost("Add")]
        public async Task Add(Product product)
        {
            await _productService.Add(product);
        }

        [HttpDelete("Delete")]
        public async Task Delete(int id)
        {
            await _productService.Delete(id);
        }       
    }
}
