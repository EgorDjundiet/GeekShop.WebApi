using GeekShop.Repositories;
using GeekShop.Domain;
namespace GeekShop.Services
{
    public interface IProductService
    {
        void Add(Product product);
        IEnumerable<Product> GetAll();
        Product? Get(int id);
        void Delete(int id);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public void Add(Product product)
        {
            _repository.Add(product);
        }

        public void Delete(int id)
        {
            if(_repository.Get(id) is not null)
            {
                _repository.Delete(id);
            }
        }

        public Product? Get(int id)
        {
            return _repository.Get(id);
        }

        public IEnumerable<Product> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
