using GeekShop.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeekShop.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SeedDataController : ControllerBase
    {
        private readonly ISeedDataService _service;
        public SeedDataController(ISeedDataService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task SeedData()
        {
            await _service.SeedData();
        }
    }
}
