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
        private readonly ICategoryRepository _categoryRepository;
        private readonly AbstractValidator<SubmitCategoryWithParentIn> _categoryValidator;
        public CategoryService(ICategoryRepository categoryRepository, AbstractValidator<SubmitCategoryWithParentIn> categoryValidator)
        {
            _categoryRepository = categoryRepository;
            _categoryValidator = categoryValidator;
        }
        public async Task<CategoryWithParent> Add(SubmitCategoryWithParentIn categoryIn)
        {
            var result = _categoryValidator.Validate(categoryIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            if (categoryIn.ParentId != null && await _categoryRepository.Get(categoryIn.ParentId!.Value) == null)
            {
                throw new NotFoundException($"Invalid parent id: {categoryIn.ParentId}");
            }

            var category = new CategoryWithParent
            {
                Name = categoryIn.Name!,
                Description = categoryIn.Description!,
                ParentId = categoryIn.ParentId
            };
            var id = await _categoryRepository.Add(category);

            var returnedCategory = await _categoryRepository.Get(id);
            return returnedCategory!;
        }

        public async Task Delete(int id)
        {
            if (_categoryRepository.Get(id) == null)
            {
                throw new NotFoundException($"Invalid category id: {id}");
            }
            await _categoryRepository.Delete(id);
        }

        public async Task<CategoryWithParent> Get(int id)
        {
            var category = await _categoryRepository.Get(id);
            if(category == null)
            {
                throw new NotFoundException($"Invalid category id: {id}");
            }
            return category;
        }

        public async Task<IEnumerable<CategoryWithParent>> GetAll()
        {
            var categories = await _categoryRepository.GetAll();
            return categories;
        }
      
        public async Task SeedData()
        {
            var submitCategories = new List<SubmitCategoryWithParentIn>()
            {
                new SubmitCategoryWithParentIn()
                {
                    Name = "Books"
                },
                new SubmitCategoryWithParentIn()
                {
                    Name = "Clothes"
                },
                new SubmitCategoryWithParentIn()
                {
                    Name = "Toys"
                },
                new SubmitCategoryWithParentIn()
                {
                    Name = "Jewellery"
                },
                new SubmitCategoryWithParentIn()
                {
                    Name = "Sporting Goods"
                },
            };

            foreach (var category in submitCategories)
            {
                await Add(category);
            }

        }
        public async Task Update(int id, SubmitCategoryWithParentIn categoryIn)
        {
            var result = _categoryValidator.Validate(categoryIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            if (await _categoryRepository.Get(id) == null)
            {
                throw new NotFoundException($"Invalid category id: {id}");
            }
            if (categoryIn.ParentId != null && await _categoryRepository.Get(categoryIn.ParentId!.Value) == null)
            {
                throw new NotFoundException($"Invalid parent id: {categoryIn.ParentId}");
            }
            var category = new CategoryWithParent
            {
                Name = categoryIn.Name!,
                Description = categoryIn.Description!,
                ParentId = categoryIn.ParentId 
            };
            await _categoryRepository.Update(id,category);
        }


        public async Task<CategoryTree> GetBreadcrumbs(int id)
        {
            var category = await _categoryRepository.Get(id);
            if (category == null)
            {
                throw new NotFoundException($"Invalid category id: {id}");
            }
            var allCategories = await _categoryRepository.GetAll();
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
            var allCategories = await _categoryRepository.GetAll();
            return new CategoryTree(allCategories);
        }


    }
}
