using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;

namespace GeekShop.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AbstractValidator<SubmitCategoryWithParentIn> _categoryValidator;
        public CategoryService(IUnitOfWork unitOfWork, AbstractValidator<SubmitCategoryWithParentIn> categoryValidator)
        {
            _unitOfWork = unitOfWork;
            _categoryValidator = categoryValidator;
        }
        public async Task<CategoryWithParent> Add(SubmitCategoryWithParentIn categoryIn)
        {
            var result = _categoryValidator.Validate(categoryIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            if (categoryIn.ParentId != null && await _unitOfWork.Categories.Get(categoryIn.ParentId!.Value) == null)
            {
                throw new NotFoundException($"Invalid parent id: {categoryIn.ParentId}");
            }

            var category = new CategoryWithParent
            {
                Name = categoryIn.Name!,
                Description = categoryIn.Description!,
                ParentId = categoryIn.ParentId
            };
            var id = await _unitOfWork.Categories.Add(category);

            category = await Get(id);
            _unitOfWork.SaveChanges();
            return category;
        }
        public async Task Update(int id, SubmitCategoryWithParentIn categoryIn)
        {
            var result = _categoryValidator.Validate(categoryIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            if (await _unitOfWork.Categories.Get(id) == null)
            {
                throw new NotFoundException($"Invalid category id: {id}");
            }
            if (categoryIn.ParentId != null && await _unitOfWork.Categories.Get(categoryIn.ParentId!.Value) == null)
            {
                throw new NotFoundException($"Invalid parent id: {categoryIn.ParentId}");
            }
            var category = new CategoryWithParent
            {
                Name = categoryIn.Name!,
                Description = categoryIn.Description!,
                ParentId = categoryIn.ParentId
            };
            await _unitOfWork.Categories.Update(id, category);
            _unitOfWork.SaveChanges();
        }

        public async Task Delete(int id)
        {
            if (_unitOfWork.Categories.Get(id) == null)
            {
                throw new NotFoundException($"Invalid category id: {id}");
            }
            await _unitOfWork.Categories.Delete(id);
            _unitOfWork.SaveChanges();
        }
        public async Task<CategoryWithParent> Get(int id)
        {
            var category = await _unitOfWork.Categories.Get(id);
            if(category == null)
            {
                throw new NotFoundException($"Invalid category id: {id}");
            }
            return category;
        }

        public async Task<IEnumerable<CategoryWithParent>> GetAll()
        {
            var categories = await _unitOfWork.Categories.GetAll();
            return categories;
        }
      
        public async Task<CategoryTree> GetBreadcrumbs(int id)
        {
            var category = await _unitOfWork.Categories.Get(id);
            if (category == null)
            {
                throw new NotFoundException($"Invalid category id: {id}");
            }
            var allCategories = await _unitOfWork.Categories.GetAll();
            var categoryWithParents = new List<CategoryWithParent> { category };
            while (category.ParentId != null)
            {
                var parent = allCategories.Where(x => x.Id == category.ParentId).Single();
                categoryWithParents.Add(parent);
                category = parent;
            }
            return new CategoryTree(categoryWithParents);
        }

        public async Task<CategoryTree> GetCategoryHierarchy()
        {
            var allCategories = await _unitOfWork.Categories.GetAll();
            return new CategoryTree(allCategories);
        }
    }
}
