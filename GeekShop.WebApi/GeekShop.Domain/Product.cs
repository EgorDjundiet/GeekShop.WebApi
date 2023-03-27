namespace GeekShop.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public static Product DefaultProduct
        {
            get => new Product
            {
                Id = -1,
                Title = "None",
                Author = "None",
                Description = "None",
            };
        }
    }
}
