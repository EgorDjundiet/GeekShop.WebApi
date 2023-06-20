namespace GeekShop.Domain
{
    public enum OrderStatus
    {
        None,
        Placed,
        PendingPayment,
        Paid,
        Shipped,
        Completed
    }
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Address CustomerAddress { get; set; } = new Address();
        public DateTime Date { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public List<OrderDetails> Details { get; set; } = new List<OrderDetails>();
        public decimal TotalCost // DO not save it to database and do not read 
        {
            get => Details.Sum(detail => detail.ProductPrice * detail.ProductQuantity);
        }
        public OrderStatus Status { get; set; }
    }
}
