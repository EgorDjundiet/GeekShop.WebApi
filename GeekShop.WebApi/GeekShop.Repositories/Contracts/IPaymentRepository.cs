using GeekShop.Domain;

namespace GeekShop.Repositories.Contracts
{
    public interface IPaymentRepository
    {
        Task<Payment?> Get(int id);
        Task<IEnumerable<Payment>> GetAll();
        Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids);
        Task<Payment> Add(Payment payment);
        Task Update(Payment payment);
        Task Delete(int id);
    }
}
