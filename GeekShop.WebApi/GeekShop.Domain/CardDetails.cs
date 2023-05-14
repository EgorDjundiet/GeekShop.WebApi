namespace GeekShop.Domain
{
    public class CardDetails
    {
        public int Id { get; set; }
        public string NameOnCard { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public DateTime ExpDate { get; set; }  // month and year (Date should be last day of month)
        public string? Cvv { get; set; }
    }
}
