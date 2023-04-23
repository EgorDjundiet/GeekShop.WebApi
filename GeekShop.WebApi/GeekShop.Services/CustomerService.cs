using GeekShop.Domain;
using GeekShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Services
{
    public interface ICustomerService
    {
        Task Add(Customer customer);
        Task Update(Customer customer);
        Task Delete(int id);
        Task<Customer?> Get(int id);
        Task<IEnumerable<Customer>> GetAll();
    }
    public class CustomerService : ICustomerService
    {
        ICustomerRepository _repository;
        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }
        public async Task Add(Customer customer)
        {
            await _repository.Add(customer);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public async Task<Customer?> Get(int id)
        {
            return await _repository.Get(id);
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task Update(Customer customer)
        {
            await _repository.Update(customer);
        }
    }
}
