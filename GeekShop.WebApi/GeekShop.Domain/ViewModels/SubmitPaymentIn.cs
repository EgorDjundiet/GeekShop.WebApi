using FluentValidation;
using System.Data;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitPaymentIn
    {
        public SubmitCardDetailsIn? CardDetails { get; set; }
        public decimal? Amount { get; set; }
        public int? OrderId { get; set; }
        public SubmitAddressIn? BillingAddress { get; set; }
    }
    public class SubmitPaymentInValidator : AbstractValidator<SubmitPaymentIn>
    {
        public SubmitPaymentInValidator(AbstractValidator<SubmitCardDetailsIn> submitCardDetailsInValidator, AbstractValidator<SubmitAddressIn> submitAddressInValidator)
        {
            RuleFor(payment => payment.CardDetails!)
                .NotEmpty().WithMessage("Card details are required")
                .SetValidator(payment => submitCardDetailsInValidator);

            RuleFor(payment => payment.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThanOrEqualTo(0).WithMessage("Invalid amount");

            RuleFor(payment => payment.OrderId)
                .NotEmpty().WithMessage("Order id is required")
                .GreaterThanOrEqualTo(1).WithMessage("Invalid order id");

            RuleFor(payment => payment.BillingAddress!)
                .NotEmpty().WithMessage("Billing address is required")
                .SetValidator(payment => submitAddressInValidator);
        }
    }
}
