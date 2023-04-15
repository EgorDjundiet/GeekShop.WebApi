using GeekShop.Domain;
using GeekShop.Services;
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
        public async Task<Order?> Get(int id)
        {
            return await _orderService.Get(id);
        }

        [HttpPost("Add")]
        public async Task Add(Order order)
        {
            await _orderService.Add(order);
        }

        [HttpDelete("Delete")]
        public async Task Delete(int id)
        {
            await _orderService.Delete(id);
        }

        [HttpPost("Update")]
        public async Task Update(Order order)
        {
            await _orderService.Update(order);
        }
    }
}
