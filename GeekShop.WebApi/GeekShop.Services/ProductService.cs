using GeekShop.Repositories;
using GeekShop.Domain;

namespace GeekShop.Services
{
    public interface IProductService
    {
        Task Add(Product product);
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> Get(int id);
        Task Delete(int id);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task Add(Product product)
        {
            await _repository.Add(product);
        }

        public async Task Delete(int id)
        {
            if(_repository.Get(id) is not null)
            {
                await _repository.Delete(id);
            }
        }

        public async Task<Product?> Get(int id)
        {
            return await _repository.Get(id);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _repository.GetAll();
        }
    }
}
