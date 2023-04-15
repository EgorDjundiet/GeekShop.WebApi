namespace GeekShop.Domain.ViewModels
{
    public class SubmitOrderIn
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? PhoneNumber { get; set; }
        public List<SubmitOrderDetailsIn> Details { get; set; } = new List<SubmitOrderDetailsIn>();        
    }
}
