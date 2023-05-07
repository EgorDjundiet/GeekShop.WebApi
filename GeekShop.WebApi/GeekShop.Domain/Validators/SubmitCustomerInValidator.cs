using FluentValidation;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Domain.Validators
{
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
