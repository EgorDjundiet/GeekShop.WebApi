namespace GeekShop.Domain
{
    public class Order
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public DateTime Date { get; set; }
        public string? PhoneNumber { get; set; }
        public List<OrderDetails> Details { get; set; }
        public decimal? TotalCost // DO not save it to database and do not read 
        {
            get => Details.Sum(detail => detail.Product.Price * detail.ProductQuantity);
        }
    }
}
