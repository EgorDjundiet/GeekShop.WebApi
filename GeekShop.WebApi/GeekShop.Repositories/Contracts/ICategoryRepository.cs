using GeekShop.Domain.ViewModels;
using GeekShop.Domain;

namespace GeekShop.Repositories.Contracts
{
    public interface ICategoryRepository
    {
        Task<int> Add(CategoryWithParent category);
        Task Update(int id, CategoryWithParent category);
        Task Delete(int id);
        Task<CategoryWithParent?> Get(int id);
        Task<IEnumerable<CategoryWithParent>> GetAll();
    }
}
