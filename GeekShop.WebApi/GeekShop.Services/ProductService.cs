using GeekShop.Repositories;
using GeekShop.Domain;
namespace GeekShop.Services
{
    public interface IProductService : ICRUD
    {

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
            _repository.Delete(id);
        }
        public Product Get(int id)
        {
            return _repository.Get(id);
        }
        public IEnumerable<Product> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
