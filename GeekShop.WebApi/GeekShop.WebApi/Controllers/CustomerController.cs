using GeekShop.Domain;
using GeekShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeekShop.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet("GetAll")]
        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _customerService.GetAll();
        }

        [HttpGet("Get")]
        public async Task<Customer?> Get(int id)
        {
            return await _customerService.Get(id);
        }

        [HttpPost("Add")]
        public async Task Add(Customer customer)
        {
            await _customerService.Add(customer);
        }

        [HttpDelete("Delete")]
        public async Task Delete(int id)
        {
            await _customerService.Delete(id);
        }

        [HttpPost("Update")]
        public async Task Update(Customer customer)
        {
            await _customerService.Update(customer);
        }
    }
}
