namespace GeekShop.Domain
{
    public class OrderDetails
    {
        public Product Product { get; set; } = new Product();
        public int ProductQuantity { get; set; }
    }
}
