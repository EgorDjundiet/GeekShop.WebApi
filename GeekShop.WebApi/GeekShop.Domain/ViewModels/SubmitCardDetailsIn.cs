using FluentValidation;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitCardDetailsIn
    {
        public string? NameOnCard { get; set; }
        public string? AccountNumber { get; set; }
        public DateTime? ExpDate { get; set; }
        public string? Cvv { get; set; }
    }
    public class SubmitCardDetailsInValidator : AbstractValidator<SubmitCardDetailsIn>
    {
        public SubmitCardDetailsInValidator()
        {
            RuleFor(card => card.NameOnCard)
                .NotEmpty().WithMessage("Name on card is required");

            RuleFor(card => card.AccountNumber)
                .NotEmpty().WithMessage("Account number is required")
                .Matches(@"^\d{4}\s?\d{4}\s?\d{4}\s?\d{4}$").WithMessage("Invalid account number");

            RuleFor(card => card.ExpDate)
                .NotEmpty().WithMessage("ExpDate is required");

            RuleFor(card => card.Cvv)               
                .Must(cvv => cvv is null || int.TryParse(cvv, out int result)).WithMessage("Invalid cvv");
        }
    }
}
