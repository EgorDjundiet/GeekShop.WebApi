using GeekShop.Domain;

namespace GeekShop.Repositories.Contracts
{
    public interface IPaymentRepository
    {
        Task<Payment?> Get(int id);
        Task<Order?> GetOrderByPaymentId(int id);
        Task<IEnumerable<Payment>> GetAll();
        Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids);
        Task<int> Add(Payment payment, Order order);
        Task Update(Payment payment, Order order);
        Task Delete(int id);
    }
}
