namespace GeekShop.Domain
{
    public class CardDetails
    {
        string NameOnCard { get; set; } = string.Empty;
        string AccountNumber { get; set; } = string.Empty;
        DateTime ExpDate { get; set; }  // month and year (Date should be last day of month)
        string? Cvv { get; set; }
    }
}
