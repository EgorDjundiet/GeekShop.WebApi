namespace GeekShop.Domain.ViewModels
{
    public class SubmitOrderIn
    {
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public List<SubmitOrderDetailsIn>? Details { get; set; }        
    }
}
