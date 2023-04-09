using GeekShop.Repositories;
using GeekShop.Domain;
using System.Diagnostics.Metrics;

namespace GeekShop.Services
{
    public interface IProductService
    {
        Task Add(Product product);
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> Get(int id);
        Task Delete(int id);
        Task PopulateDb();
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository repository)
        {
            _productRepository = repository;
        }

        public async Task Add(Product product)
        {
            await _productRepository.Add(product);
        }

        public async Task Delete(int id)
        {
            if(_productRepository.Get(id) is not null)
            {
                await _productRepository.Delete(id);
            }
        }

        public async Task<Product?> Get(int id)
        {
            return await _productRepository.Get(id);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productRepository.GetAll();
        }


        public async Task PopulateDb()
        {
            List<Product> list = new()
            {
                new Product
                {
                    Title = "Harry Potter and the Philosopher's Stone",
                    Author = "J.K.Rowling",
                    Description = "The story of the boy who survived"
                },
                new Product
                {
                    Title = "War and Peace",
                    Author = "Lev Tolstoy",
                    Description = "The рistorical novel about war and peace"
                },
                new Product
                {

                    Title = "The Catcher in the Rye",
                    Author = "J.D.Salinger",
                    Description ="The story of the teen who found a purpose in his life"
                }
            };
            
            foreach (var product in list)
            {
                await _productRepository.Add(product);
            }
        }
    }
}
