using GeekShop.Domain.ViewModels;
using GeekShop.Domain;

namespace GeekShop.Services.Contracts
{
    public interface ICategoryService
    {
        Task SeedData();
        Task<CategoryWithParent> Add(SubmitCategoryWithParentIn categoryIn);
        Task Update(int id, SubmitCategoryWithParentIn categoryIn);
        Task Delete(int id);
        Task<CategoryWithParent> Get(int id);
        Task<IEnumerable<CategoryWithParent>> GetAll();
        Task<CategoryTree> GetCategoryHierarchy();
        Task<CategoryTree> GetBreadcrumbs(int id);
    }
}
