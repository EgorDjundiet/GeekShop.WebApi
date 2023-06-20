namespace GeekShop.Domain
{
    public class CatHierarchy
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public int? ParentId { get; set; }
    }
}
