namespace GeekShop.Domain
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } = new Address();
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
