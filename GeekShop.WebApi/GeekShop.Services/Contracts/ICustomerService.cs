using GeekShop.Domain;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Services.Contracts
{
    public interface ICustomerService
    {
        Task<Customer> Add(SubmitCustomerIn customerIn);
        Task Update(int id, SubmitCustomerIn customer);
        Task Delete(int id);
        Task<Customer> Get(int id);
        Task<IEnumerable<Customer>> GetAll();
        Task<IEnumerable<Customer>> GetByIds(IEnumerable<int> ids);
    }
}
