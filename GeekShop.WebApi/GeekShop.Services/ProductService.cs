﻿using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;

namespace GeekShop.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly AbstractValidator<SubmitProductIn> _productValidator;
        public ProductService(IProductRepository repository, ICategoryRepository categoryRepository, AbstractValidator<SubmitProductIn> productValidator)
        {
            _productRepository = repository;
            _categoryRepository = categoryRepository;
            _productValidator = productValidator;
        }

        public async Task<Product> Add(SubmitProductIn productIn)
        {
            var result = _productValidator.Validate(productIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }

            var product = new Product()
            {
                Title = productIn.Title!,
                Author = productIn.Author!,
                Description = productIn.Description,
                Price = productIn.Price!.Value,
                CategoryName = (await _categoryRepository.Get(productIn.CategoryId!.Value))!.Name
            };
            var id = await _productRepository.Add(product);
            return await Get(id);
        }

        public async Task Delete(int id)
        {
            var product = await _productRepository.Get(id);
            if (product is null)
            {
                throw new NotFoundException($"Invalid product id {id}");
            }
            await _productRepository.Delete(id);
        }

        public async Task<Product> Get(int id)
        {
            var product = await _productRepository.Get(id);
            if (product is null)
            {
                throw new NotFoundException($"Invalid product id {id}");
            }
            return product;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productRepository.GetAll();
        }

        public async Task<IEnumerable<Product>> GetByIds(IEnumerable<int> ids)
        {
            ids = ids.Distinct();   
            var products = await _productRepository.GetByIds(ids);
            var invalidIds = ids.Where(id => !products.Select(p => p.Id).Contains(id));
            if (invalidIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidIds);
                throw new NotFoundException($"Invalid product ids {unfoundIds}");
            }
            return await _productRepository.GetByIds(ids);
        }
        public async Task SeedData()
        {
            var products = new List<SubmitProductIn>()
            {
                new SubmitProductIn
                { 
                    Title = "Harry Potter And The Philosopher's Stone",
                    Author = "J.K.Rowling",
                    Description = "The story of the boy who lived",
                    Price = 13,
                    CategoryId = 1
                },
                new SubmitProductIn
                {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Description = "A powerful story of racial injustice and moral growth in a small Southern town",
                    Price = 20,
                    CategoryId = 1
                },
                new SubmitProductIn
                {
                    Title = "1984",
                    Author = "George Orwell",
                    Description = "A dystopian novel depicting a totalitarian society where Big Brother watches every move",
                    Price = 7.99M,
                    CategoryId = 1
                },
                new SubmitProductIn
                {
                    Title = "The Catcher in the Rye",
                    Author = "J.D.Salinger",
                    Description = "A coming-of-age novel following a disillusioned teenager's journey through New York City",
                    Price = 17,
                    CategoryId = 1
                },
                new SubmitProductIn
                {
                    Title = "To the Lighthouse",
                    Author = "Virginia Woolf",
                    Description = "A stream-of-consciousness novel exploring the intricacies of human emotions and relationships",
                    Price = 10,
                    CategoryId = 1
                }
            };

            foreach (var product in products)
            {
                await Add(product);
            }    
        }

        public async Task Update(int id, SubmitProductIn productIn)
        {
            var result = _productValidator.Validate(productIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            var product = await _productRepository.Get(id);
            if (product is null)
            {
                throw new NotFoundException($"Invalid product id {id}");
            }
            product.Title = productIn.Title!;
            product.Author = productIn.Author!;
            product.Description = productIn.Description; 
            product.Price = productIn.Price!.Value;
            product.CategoryName = (await _categoryRepository.Get(productIn.CategoryId!.Value))!.Name;

            await _productRepository.Update(product);
        }      
    }
}
