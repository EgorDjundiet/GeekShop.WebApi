using FluentValidation;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Domain.Validators
{
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
