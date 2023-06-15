namespace GeekShop.Domain
{
    public class CategoryWithParent : Category
    {
        public int? ParentId { get; set; }
    }
}
