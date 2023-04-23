using GeekShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Repositories
{
    public interface ICustomerRepository
    {
        Task Add(Customer customer);
        Task Update(Customer customer);
        Task Delete(int id);
        Task<Customer?> Get(int id);
        Task<IEnumerable<Customer>> GetAll();
    }
}
