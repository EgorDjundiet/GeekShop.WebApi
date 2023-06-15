using Azure.Core;
using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;

namespace GeekShop.Services
{  
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AbstractValidator<SubmitCustomerIn> _customerValidator;
        public CustomerService(IUnitOfWork unitOfWork, AbstractValidator<SubmitCustomerIn> customerValidator)
        {
            _unitOfWork = unitOfWork;
            _customerValidator = customerValidator;
        }
        public async Task<Customer> Add(SubmitCustomerIn customerIn)
        {
            var result = _customerValidator.Validate(customerIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }

            var customer = new Customer()
            {
                Name = customerIn.Name!,
                Address = new Address()
                {
                    Street = customerIn.Address!.Street!,
                    City = customerIn.Address!.City!,
                    State = customerIn.Address!.State!,
                    ZipCode = customerIn.Address!.ZipCode!,
                    Country = customerIn.Address!.Country!
                },
                PhoneNumber = customerIn.PhoneNumber,
                Email = customerIn.Email
            };
            var id = await _unitOfWork.Customers.Add(customer);
            customer = await Get(id);
            _unitOfWork.SaveChanges();
            return customer;
        }
        public async Task Delete(int id)
        {
            var customer = await _unitOfWork.Customers.Get(id);
            if (customer is null)
            {
                throw new NotFoundException($"Invalid customer id: {id}");
            }
            await _unitOfWork.Customers.Delete(id);
            _unitOfWork.SaveChanges();
        }
        public async Task Update(int id, SubmitCustomerIn customerIn)
        {
            var result = _customerValidator.Validate(customerIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }

            var customer = await _unitOfWork.Customers.Get(id);
            if (customer is null)
            {
                throw new NotFoundException($"Invalid customer id: {id}");
            }

            customer.Name = customerIn.Name!;
            customer.Address = new Address()
            {
                Street = customerIn.Address!.Street!,
                City = customerIn.Address!.City!,
                State = customerIn.Address!.State!,
                ZipCode = customerIn.Address!.ZipCode!,
                Country = customerIn.Address!.Country!
            };
            customer.PhoneNumber = customerIn.PhoneNumber;
            customer.Email = customerIn.Email;
            await _unitOfWork.Customers.Update(customer);
            _unitOfWork.SaveChanges();
        }
        public async Task<Customer> Get(int id)
        {
            var customer = await _unitOfWork.Customers.Get(id);
            if (customer is null)
            {
                throw new NotFoundException($"Invalid customer id: {id}");
            }
            return customer;
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _unitOfWork.Customers.GetAll();
        }

        public async Task<IEnumerable<Customer>> GetByIds(IEnumerable<int> ids)
        {
            ids = ids.Distinct();
            var customers = await _unitOfWork.Customers.GetByIds(ids);
            var invalidIds = ids.Where(id => !customers.Select(p => p.Id).Contains(id));
            if (invalidIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidIds);
                throw new NotFoundException($"Invalid customer ids: {unfoundIds}");
            }
            return customers;
        }
    }
}
