using GeekShop.Domain.ViewModels;
using GeekShop.Domain;

namespace GeekShop.Services.Contracts
{
    public interface IProductService
    {
        Task Add(SubmitProductIn productIn);
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetByIds(IEnumerable<int> ids);
        Task<Product?> Get(int id);
        Task Delete(int id);
        Task Update(int id, SubmitProductIn productIn);
    }
}
