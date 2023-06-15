using GeekShop.Domain;
using GeekShop.Domain.ViewModels;
using GeekShop.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeekShop.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _orderService.GetAll();
        }

        [HttpGet("Get")]
        public async Task<Order> Get(int id)
        {
            return await _orderService.Get(id);
        }

        [HttpPost("GetByIds")]
        public async Task<IEnumerable<Order>> GetByIds([FromBody] IEnumerable<int> ids)
        {
            return await _orderService.GetByIds(ids);
        }

        [HttpPost("Add")]
        public async Task<Order> Add([FromBody] SubmitOrderIn orderIn)
        {
            return await _orderService.Add(orderIn);                    
        }
        [HttpPut("Update")]
        public async Task Update(int id, [FromBody] SubmitOrderIn orderIn)
        {
            await _orderService.Update(id, orderIn);
        }
        [HttpDelete("Delete")]
        public async Task Delete(int id)
        {
            await _orderService.Delete(id);
        }       
    }
}
