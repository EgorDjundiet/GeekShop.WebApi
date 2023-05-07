namespace GeekShop.Domain
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public List<OrderDetails> Details { get; set; } = new List<OrderDetails>();
        public decimal TotalCost // DO not save it to database and do not read 
        {
            get => Details.Sum(detail => detail.Product.Price * detail.ProductQuantity);
        }
    }
}
