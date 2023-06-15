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
        private readonly IUnitOfWork _unitOfWork;
        private readonly AbstractValidator<SubmitProductIn> _productValidator;
        public ProductService(IUnitOfWork unitOfWork, AbstractValidator<SubmitProductIn> productValidator)
        {
            _unitOfWork = unitOfWork;
            _productValidator = productValidator;
        }
        
        public async Task<Product> Add(SubmitProductIn productIn)
        {
            var result = _productValidator.Validate(productIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            var category = await _unitOfWork.Categories.Get(productIn.CategoryId!.Value);
            if(category == null)
            {
                throw new NotFoundException($"Invalid category id: {productIn.CategoryId!.Value}");
            }

            var product = new Product()
            {
                Title = productIn.Title!,
                Author = productIn.Author!,
                Description = productIn.Description,
                Price = productIn.Price!.Value,
                CategoryName = category.Name
            };
            var id = await _unitOfWork.Products.Add(product);
            product = await Get(id);
            _unitOfWork.SaveChanges();
            return product;
        }
        public async Task Delete(int id)
        {
            var product = await _unitOfWork.Products.Get(id);
            if (product is null)
            {
                throw new NotFoundException($"Invalid product id {id}");
            }
            await _unitOfWork.Products.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public async Task<Product> Get(int id)
        {
            var product = await _unitOfWork.Products.Get(id);
            if (product is null)
            {
                throw new NotFoundException($"Invalid product id {id}");
            }
            return product;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _unitOfWork.Products.GetAll();
        }

        public async Task<IEnumerable<Product>> GetByIds(IEnumerable<int> ids)
        {
            ids = ids.Distinct();   
            var products = await _unitOfWork.Products.GetByIds(ids);
            var invalidIds = ids.Where(id => !products.Select(p => p.Id).Contains(id));
            if (invalidIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidIds);
                throw new NotFoundException($"Invalid product ids {unfoundIds}");
            }
            return await _unitOfWork.Products.GetByIds(ids);
        }

        public async Task Update(int id, SubmitProductIn productIn)
        {
            var result = _productValidator.Validate(productIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            var product = await _unitOfWork.Products.Get(id);
            if (product is null)
            {
                throw new NotFoundException($"Invalid product id {id}");
            }
            var category = await _unitOfWork.Categories.Get(productIn.CategoryId!.Value);
            if (category == null)
            {
                throw new NotFoundException($"Invalid category id: {productIn.CategoryId!.Value}");
            }
            product.Title = productIn.Title!;
            product.Author = productIn.Author!;
            product.Description = productIn.Description; 
            product.Price = productIn.Price!.Value;
            product.CategoryName = category.Name;

            await _unitOfWork.Products.Update(product);
            _unitOfWork.SaveChanges();
        }      
    }
}
