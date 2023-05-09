using FluentValidation;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitOrderIn
    {
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public List<SubmitOrderDetailsIn>? Details { get; set; }        
    }
    public class SumbitOrderInValidator : AbstractValidator<SubmitOrderIn>
    {
        public SumbitOrderInValidator(AbstractValidator<SubmitOrderDetailsIn> orderDetailsValidator)
        {
            RuleFor(order => order.Details)
                .NotEmpty().WithMessage("Details are required");

            RuleForEach(order => order.Details)
                .SetValidator(details => orderDetailsValidator);

            When(order => order.CustomerId is null, () =>
            {
                RuleFor(order => order.CustomerName)
                    .NotEmpty().WithMessage("Customer name is required.");

                RuleFor(order => order.CustomerAddress)
                    .NotEmpty().WithMessage("Customer address is required.");

                RuleFor(order => order.PhoneNumber)
                    .Matches(@"^\+(?:[0-9]●?){6,14}[0-9]$").WithMessage("Invalid phone number format.");

                RuleFor(order => order.Email)
                    .EmailAddress().WithMessage("Invalid email address format.");
            });
        }
    }
}
