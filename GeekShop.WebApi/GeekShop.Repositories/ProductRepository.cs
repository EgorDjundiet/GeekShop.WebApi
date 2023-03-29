using GeekShop.Domain;
namespace GeekShop.Repositories
{
    public interface IProductRepository
    {
        Task Add(Product product);
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> Get(int id);
        Task Delete(int id);
    }
    public class ProductRepository : IProductRepository
    {
        private static List<Product> _products = new List<Product> 
        {
            new Product
            {
                Id = ++IdCounter,
                Title = "Harry Potter and the Philosopher's Stone",
                Author = "J.K.Rowling",
                Description = "The story of the boy who survived"
            },
            new Product
            {
                Id = ++IdCounter,
                Title = "War and Peace",
                Author = "Lev Tolstoy",
                Description = "The рistorical novel about war and peace"
            },
            new Product
            {
                Id = ++IdCounter,
                Title = "The Catcher in the Rye",
                Author = "J.D.Salinger",
                Description ="The story of the teen who found a purpose in his life"
            }
        };
        private static int IdCounter;
        public async Task Add(Product product)
        {
            product.Id = ++IdCounter;
            _products.Add(product);
        }

        public async Task Delete(int id)
        {
            _products.Remove(_products.First(p => p.Id == id));
        }

        public async Task<Product?> Get(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return _products;
        }
    }
}
