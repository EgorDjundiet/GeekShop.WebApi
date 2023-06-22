using GeekShop.Domain;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Services.Contracts
{
    public interface IPaymentService
    {
        Task SeedData();
        Task<Payment> Get(int id);
        Task<IEnumerable<Payment>> GetAll();
        Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids);
        Task<Payment> Add(SubmitPaymentIn payment);
        Task Update(int id, SubmitPaymentIn payment);
        Task Delete(int id);
    }
}
