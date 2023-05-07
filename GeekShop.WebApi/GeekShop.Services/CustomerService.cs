using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Services
{
    
    public class CustomerService : ICustomerService
    {
        ICustomerRepository _customerRepository;
        AbstractValidator<Customer> _customerValidator;
        public CustomerService(ICustomerRepository customerRepository, AbstractValidator<Customer> customerValidator)
        {
            _customerRepository = customerRepository;
            _customerValidator = customerValidator;
        }
        public async Task Add(SubmitCustomerIn customerIn)
        {
            var customer = new Customer() { Name = customerIn.Name, Address = customerIn.Address, PhoneNumber = customerIn.PhoneNumber, Email = customerIn.Email};
            var result = _customerValidator.Validate(customer);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }

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

        public async Task<Customer?> Get(int id)
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

        public async Task<IEnumerable<Customer?>> GetByIds(IEnumerable<int> ids)
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
            var customer = await _customerRepository.Get(id);
            if (customer is null)
            {
                throw new GeekShopNotFoundException($"Invalid customer id: {id}");
            }
            customer.Name = customerIn.Name;
            customer.Address = customerIn.Address;
            customer.PhoneNumber = customerIn.PhoneNumber;
            customer.Email = customerIn.Email;

            var result = _customerValidator.Validate(customer);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }

            await _customerRepository.Update(customer);
        }
    }
}
