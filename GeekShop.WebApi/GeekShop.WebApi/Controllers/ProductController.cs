using GeekShop.Domain;
using GeekShop.Repositories;
using GeekShop.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

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
        public IEnumerable<Product> GetAll()
        {
            return _productService.GetAll();
        }

        [HttpGet("Get")]
        public Product? Get(int id)
        {
            return _productService.Get(id);
        }

        [HttpPost("Add")]
        public void Add(Product product)
        {
            _productService.Add(product);
        }

        [HttpDelete("Delete")]
        public void Delete(int id)
        {
            _productService.Delete(id);
        }       
    }
}
