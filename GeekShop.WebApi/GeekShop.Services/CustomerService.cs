﻿using FluentValidation;
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
        private readonly ICustomerRepository _customerRepository;
        private readonly AbstractValidator<SubmitCustomerIn> _customerValidator;
        public CustomerService(ICustomerRepository customerRepository, AbstractValidator<SubmitCustomerIn> customerValidator)
        {
            _customerRepository = customerRepository;
            _customerValidator = customerValidator;
        }
        public async Task SeedData()
        {
            var customers = new List<SubmitCustomerIn>() 
            { 
                new SubmitCustomerIn() 
                { 
                    Name = "John Walker", 
                    Address = new SubmitAddressIn 
                    { 
                        Street = "Privet",
                        City = "London",
                        State = "London",
                        ZipCode = "24017",
                        Country = "UK"
                    },
                    PhoneNumber = "+4477890345",
                    Email = "JohnWalker@gmail.com"
                },
                new SubmitCustomerIn()
                {
                    Name = "Natalya Pushkin",
                    Address = new SubmitAddressIn
                    {
                        Street = "Komsomolskaya",
                        City = "Moscow",
                        State = "Moscow",
                        ZipCode = "46712",
                        Country = "Russia"
                    },
                    PhoneNumber = "+777623908",
                    Email = "NatalyaPushkin@gmail.com"
                },
                new SubmitCustomerIn()
                {
                    Name = "Aloys Ramstein",
                    Address = new SubmitAddressIn
                    {
                        Street = "Oderbergerstrasse",
                        City = "Berlin",
                        State = "Berlin",
                        ZipCode = "09321",
                        Country = "Germany"
                    },
                    PhoneNumber = "+4977521653",
                    Email = "AloysRamstein@gmail.com"
                }
            };

            foreach(var customer in customers)
            {
                await Add(customer);
            }
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
            var id = await _customerRepository.Add(customer);
            return await Get(id);
        }

        public async Task Delete(int id)
        {
            var customer = await _customerRepository.Get(id);
            if(customer is null)
            {
                throw new NotFoundException($"Invalid customer id: {id}");
            }
            await _customerRepository.Delete(id);
        }

        public async Task<Customer> Get(int id)
        {
            var customer = await _customerRepository.Get(id);
            if (customer is null)
            {
                throw new NotFoundException($"Invalid customer id: {id}");
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
                throw new NotFoundException($"Invalid customer ids: {unfoundIds}");
            }
            return customers;
        }

        public async Task Update(int id, SubmitCustomerIn customerIn)
        {
            var result = _customerValidator.Validate(customerIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }

            var customer = await _customerRepository.Get(id);
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
            await _customerRepository.Update(customer);
        }
    }
}
