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
        int Id { get; set; }
        CardDetails CardDetails { get; set; } = new CardDetails();
        decimal Amount { get; set; }
        PaymentStatus Status { get; set; }
        int OrderId { get; set; }
        Address BillingAddress { get; set; } = new Address();
    }     
}
