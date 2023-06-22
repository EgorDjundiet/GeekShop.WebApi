using GeekShop.Domain;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Services.Contracts
{
    public interface IOrderService
    {
        Task SeedData();
        Task<Order> Add(SubmitOrderIn orderIn);
        Task<IEnumerable<Order>> GetAll();
        Task<Order> Get(int id);
        Task<IEnumerable<Order>> GetByIds(IEnumerable<int> ids);
        Task Delete(int id);
        Task Update(int id, SubmitOrderIn orderIn);
        Task ChangeStatus(int id, OrderStatus status);
    }
}
