using GeekShop.Domain;
using GeekShop.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Services
{
    public interface IOrderService
    {
        Task Add(Order order);
        Task<IEnumerable<Order>> GetAll();
        Task<Order?> Get(int id);
        Task Delete(int id);
        Task Update(Order order);
    }
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository repository, IProductRepository productRepository)
        {
            _orderRepository = repository;
            _productRepository = productRepository;
        }
        public async Task Add(Order order)
        {
            var productList = await _productRepository.GetByIds(order.Details.Select(d => d.ProductId));
            if (productList.Count() != order.Details.Select(x => x.ProductId).Distinct().Count())
            {
                throw new ArgumentException("Invalid ProductId");
            }
            /*
             Convert submitOrderin to Order (use productList)
             */
            await _orderRepository.Add(order);
        }

        public async Task Delete(int id)
        {
            await _orderRepository.Delete(id);
        }

        public async Task<Order?> Get(int id)
        {
            return await _orderRepository.Get(id);
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _orderRepository.GetAll();
        }

        public async Task Update(Order order)
        {
            await _orderRepository.Update(order);
        }
    }
}
