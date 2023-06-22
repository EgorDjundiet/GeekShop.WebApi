namespace GeekShop.Domain
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string ProductTitle { get; set; } = string.Empty;
        public string ProductAuthor { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
    }
}
