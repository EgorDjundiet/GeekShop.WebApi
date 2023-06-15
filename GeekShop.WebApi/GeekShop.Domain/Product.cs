using System.ComponentModel.DataAnnotations;

namespace GeekShop.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
