using GeekShop.Domain;
using GeekShop.Domain.ViewModels;
using GeekShop.Services.Contracts;
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
        [HttpGet("SeedData")]
        public async Task SeedData()
        {
            await _customerService.SeedData();
        }
        [HttpGet("GetAll")]
        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _customerService.GetAll();
        }
        
        [HttpGet("Get")]
        public async Task<Customer> Get(int id)
        {
            return await _customerService.Get(id);
        }
        [HttpPost("GetByIds")]
        public async Task<IEnumerable<Customer>> GetByIds([FromBody] IEnumerable<int> ids)
        {
            return await _customerService.GetByIds(ids);
        }     
        [HttpPost("Add")]
        public async Task<Customer> Add([FromBody]SubmitCustomerIn customer)
        {
            return await _customerService.Add(customer);           
        }
        [HttpPut("Update")]
        public async Task Update(int id, [FromBody] SubmitCustomerIn customer)
        {
            await _customerService.Update(id, customer);
        }
        [HttpDelete("Delete")]
        public async Task Delete(int id)
        {
            await _customerService.Delete(id);
        }       
    }
}
