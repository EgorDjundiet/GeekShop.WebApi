using GeekShop.Domain;
using GeekShop.Domain.ViewModels;
namespace GeekShop.Repositories.Contracts
{
    public interface IProductRepository
    {
        Task<Product> Add(Product product);
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> Get(int id);
        Task Delete(int id);
        Task Update(Product product);
        Task<IEnumerable<Product>> GetByIds(IEnumerable<int> ids);
    }
}