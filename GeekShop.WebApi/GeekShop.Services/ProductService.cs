using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;

namespace GeekShop.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly AbstractValidator<SubmitProductIn> _productValidator;
        public ProductService(IProductRepository repository, AbstractValidator<SubmitProductIn> productValidator)
        {
            _productRepository = repository;
            _productValidator = productValidator;
        }

        public async Task Add(SubmitProductIn productIn)
        {
            var result = _productValidator.Validate(productIn);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }

            var product = new Product() { Title = productIn.Title, Author = productIn.Author, Description = productIn.Description, Price = productIn.Price.Value};
            await _productRepository.Add(product);
        }

        public async Task Delete(int id)
        {
            var product = await _productRepository.Get(id);
            if (product is null)
            {
                throw new GeekShopNotFoundException($"Invalid product id {id}");
            }
            await _productRepository.Delete(id);         
        }

        public async Task<Product> Get(int id)
        {
            var product = await _productRepository.Get(id);
            if (product is null)
            {
                throw new GeekShopNotFoundException($"Invalid product id {id}");
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
                throw new GeekShopNotFoundException($"Invalid product ids {unfoundIds}");
            }
            return await _productRepository.GetByIds(ids);
        }

        public async Task Update(int id, SubmitProductIn productIn)
        {
            var result = _productValidator.Validate(productIn);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }
            var product = await _productRepository.Get(id);
            if (product is null)
            {
                throw new GeekShopNotFoundException($"Invalid product id {id}");
            }
            product.Title = productIn.Title;
            product.Author = productIn.Author;
            product.Description = productIn.Description; 
            product.Price = productIn.Price.Value;

            
            await _productRepository.Update(product);
        }      
    }
}
