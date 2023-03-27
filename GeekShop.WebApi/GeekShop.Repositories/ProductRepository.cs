using GeekShop.Domain;
namespace GeekShop.Repositories
{
    public interface IProductRepository : ICRUD
    {
        public static IEnumerable<Product> Products { get; set; }
    }
    public class ProductRepository : IProductRepository
    {
        public static IEnumerable<Product> Products
        {
            get => products;
            set => products = value;
        }
        private static IEnumerable<Product> products = new List<Product> 
        {
            new Product
            {
                Id = 1,
                Title = "Harry Potter and the Philosopher's Stone",
                Author = "J.K.Rowling",
                Description = "The story of the boy who survived"
            },
            new Product
            {
                Id = 2,
                Title = "War and Peace",
                Author = "Lev Tolstoy",
                Description = "The рistorical novel about war and peace"
            },
            new Product
            {
                Id = 3,
                Title = "The Catcher in the Rye",
                Author = "J.D.Salinger",
                Description ="The story of the teen who found a purpose in his life"
            }
        };
        private static int IdCounter = 3;
        public void Add(Product product)
        {
            product.Id = ++IdCounter;
            Products = Products.Append(product);
        }

        public void Delete(int id)
        {
            Products = Products.Where(p => p.Id != id);
        }

        public Product Get(int id)
        {
            return Products.FirstOrDefault(p => p.Id == id, Product.DefaultProduct);
        }

        public IEnumerable<Product> GetAll()
        {
            return Products;
        }
    }
}
