namespace GeekShop.Domain
{
    public enum PaymentStatus
    {
        None = 0,
        Submitted = 1,
        Settled = 2,
        Declined = 3,
        Error = 4
    }
    public class Payment
    {
        public int Id { get; set; }
        public CardDetails CardDetails { get; set; } = new CardDetails();
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public int OrderId { get; set; }
        public Address BillingAddress { get; set; } = new Address();
    }     
}
