namespace GeekShop.Domain
{
    public class CategoryTree
    {
        public Category Category { get; set; }
        public List<CategoryTree> ChildCategories { get; set; } = new List<CategoryTree>();
 
        public CategoryTree(IEnumerable<CategoryWithParent> allCategories)
        {
            Category = new Category 
            { 
                Id = 0,
                Name = "Root"                
            };

            ChildCategories = allCategories.Where(x => x.ParentId == null).Select(x => new CategoryTree(x, allCategories)).ToList();
        }

        public CategoryTree(Category category, IEnumerable<CategoryWithParent> allCategories)
        {
            Category = category;
            var childCategories = allCategories.Where(x => x.ParentId == category.Id);
            ChildCategories = childCategories.Select(x => new CategoryTree(x, allCategories)).ToList();
        }
    }
}
