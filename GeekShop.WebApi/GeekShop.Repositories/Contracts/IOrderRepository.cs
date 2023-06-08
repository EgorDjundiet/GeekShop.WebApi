using GeekShop.Domain;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Repositories.Contracts
{
    public interface IOrderRepository
    {
        Task<Order> Add(Order order);
        Task<IEnumerable<Order>> GetAll();
        Task<IEnumerable<Order>> GetByIds(IEnumerable<int> ids);
        Task<Order?> Get(int id);
        Task Delete(int id);
        Task Update(Order order);
    }
}
