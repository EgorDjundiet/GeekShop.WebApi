using FluentValidation;
using GeekShop.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Domain.Validators
{
    public class SubmitOrderDetailsInValidator : AbstractValidator<SubmitOrderDetailsIn>
    {
        public SubmitOrderDetailsInValidator()
        {
            RuleFor(orderDetails => orderDetails.ProductId)
                .NotEmpty().WithMessage("Product id is required");

            RuleFor(orderDetails => orderDetails.ProductQuantity)
                .NotEmpty().WithMessage("Product quantity is required");
        }
    }
}
