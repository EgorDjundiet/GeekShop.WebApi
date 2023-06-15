using GeekShop.Domain;

namespace GeekShop.Repositories.Contracts
{
    public interface ICustomerRepository
    {
        Task<int> Add(Customer customer);
        Task Update(Customer customer);
        Task Delete(int id);
        Task<Customer?> Get(int id);
        Task<IEnumerable<Customer>> GetAll();
        Task<IEnumerable<Customer>> GetByIds(IEnumerable<int> ids);
    }
}
