namespace GeekShop.Domain.Settings
{
    public class DbOptions 
    {
        public const string BasePosition = "DbSettings";
        public string? ConnectionString { get; set; }
    }
}
