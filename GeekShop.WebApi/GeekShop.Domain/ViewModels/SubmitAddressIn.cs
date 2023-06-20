using FluentValidation;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitAddressIn
    {
        public string? Street { get; set; } 
        public string? City { get; set; } 
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
    }
    public class SubmitAddressInValidator : AbstractValidator<SubmitAddressIn>
    {
        public SubmitAddressInValidator()
        {
            RuleFor(address => address.Street)
                .NotEmpty().WithMessage("Street is required");

            RuleFor(address => address.City)
                .NotEmpty().WithMessage("City is required");

            RuleFor(address => address.State)
                .NotEmpty().WithMessage("State is required");

            RuleFor(address => address.ZipCode)
                .NotEmpty().WithMessage("ZipCode is required");

            RuleFor(address => address.Country)
                .NotEmpty().WithMessage("Country is required");
        }
    }
}
