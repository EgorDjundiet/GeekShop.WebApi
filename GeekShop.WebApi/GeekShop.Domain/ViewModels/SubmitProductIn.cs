using FluentValidation;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitProductIn
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
    }
    public class SubmitProductInValidator : AbstractValidator<SubmitProductIn>
    {
        public SubmitProductInValidator()
        {
            RuleFor(product => product.Title)
                .NotEmpty().WithMessage("Title is required");

            RuleFor(product => product.Author)
                .NotEmpty().WithMessage("Author is required");

            RuleFor(product => product.Price)
                .NotEmpty().WithMessage("Price is required")
                .GreaterThan(0).WithMessage("Invalid price");
        }
    }
}
