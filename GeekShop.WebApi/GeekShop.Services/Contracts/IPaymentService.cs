using GeekShop.Domain;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Services.Contracts
{
    public interface IPaymentService
    {
        Task<Payment> Get(int id);
        Task<IEnumerable<Payment>> GetAll();
        Task<Order> GetOrderByPaymentId(int id);
        Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids);
        Task<Payment> Add(SubmitPaymentIn paymentIn);
        Task Update(int id, SubmitPaymentIn payment);
        Task Delete(int id);
    }
}
