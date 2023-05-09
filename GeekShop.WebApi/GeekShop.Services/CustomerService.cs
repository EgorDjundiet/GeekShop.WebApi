using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;
namespace GeekShop.Services
{
    
    public class CustomerService : ICustomerService
    {
        ICustomerRepository _customerRepository;
        AbstractValidator<SubmitCustomerIn> _customerValidator;
        public CustomerService(ICustomerRepository customerRepository, AbstractValidator<SubmitCustomerIn> customerValidator)
        {
            _customerRepository = customerRepository;
            _customerValidator = customerValidator;
        }
        public async Task Add(SubmitCustomerIn customerIn)
        {
            var result = _customerValidator.Validate(customerIn);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }

            var customer = new Customer() { Name = customerIn.Name!, Address = customerIn.Address!, PhoneNumber = customerIn.PhoneNumber, Email = customerIn.Email };            
            await _customerRepository.Add(customer);
        }

        public async Task Delete(int id)
        {
            var customer = await _customerRepository.Get(id);
            if(customer is null)
            {
                throw new GeekShopNotFoundException($"Invalid customer id: {id}");
            }
            await _customerRepository.Delete(id);
        }

        public async Task<Customer> Get(int id)
        {
            var customer = await _customerRepository.Get(id);
            if (customer is null)
            {
                throw new GeekShopNotFoundException($"Invalid customer id: {id}");
            }
            return customer;
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _customerRepository.GetAll();
        }

        public async Task<IEnumerable<Customer>> GetByIds(IEnumerable<int> ids)
        {
            ids = ids.Distinct();
            var customers = await _customerRepository.GetByIds(ids);
            var invalidIds = ids.Where(id => !customers.Select(p => p.Id).Contains(id));
            if (invalidIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidIds);
                throw new GeekShopNotFoundException($"Invalid customer ids: {unfoundIds}");
            }
            return customers;
        }

        public async Task Update(int id, SubmitCustomerIn customerIn)
        {
            var result = _customerValidator.Validate(customerIn);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }

            var customer = await _customerRepository.Get(id);
            if (customer is null)
            {
                throw new GeekShopNotFoundException($"Invalid customer id: {id}");
            }

            customer.Name = customerIn.Name!;
            customer.Address = customerIn.Address!;
            customer.PhoneNumber = customerIn.PhoneNumber;
            customer.Email = customerIn.Email;           
            await _customerRepository.Update(customer);
        }
    }
}
