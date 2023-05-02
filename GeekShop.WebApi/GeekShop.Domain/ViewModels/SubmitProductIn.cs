namespace GeekShop.Domain.ViewModels
{
    public class SubmitProductIn
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
