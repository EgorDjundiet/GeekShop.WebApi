using GeekShop.Domain;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Services.Contracts
{
    public interface ICustomerService
    {
        Task Add(SubmitCustomerIn customer);
        Task Update(int id, SubmitCustomerIn customer);
        Task Delete(int id);
        Task<Customer> Get(int id);
        Task<IEnumerable<Customer>> GetAll();
        Task<IEnumerable<Customer>> GetByIds(IEnumerable<int> ids);
    }
}
