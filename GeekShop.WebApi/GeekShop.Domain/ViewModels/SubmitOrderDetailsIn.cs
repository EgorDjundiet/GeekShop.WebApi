using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitOrderDetailsIn
    {
        public int? ProductId { get; set; }
        public int? ProductQuantity { get; set; }
    }
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
