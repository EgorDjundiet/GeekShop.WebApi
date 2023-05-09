using FluentValidation;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitCustomerIn
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
    public class SubmitCustomerInValidator : AbstractValidator<SubmitCustomerIn>
    {
        public SubmitCustomerInValidator()
        {
            RuleFor(customer => customer.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(customer => customer.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(order => order.PhoneNumber)
                .Matches(@"^\+(?:[0-9]●?){6,14}[0-9]$").WithMessage("Invalid phone number format.");

            RuleFor(customer => customer.Email)
                .EmailAddress().WithMessage("Invalid email address format.");

        }
    }
}
