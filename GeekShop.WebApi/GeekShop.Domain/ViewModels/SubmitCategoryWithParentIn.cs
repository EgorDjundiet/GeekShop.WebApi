using FluentValidation;

namespace GeekShop.Domain.ViewModels
{
    public class SubmitCategoryWithParentIn
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
    }
    public class SubmitCategoryWithParentInValidator : AbstractValidator<SubmitCategoryWithParentIn>
    {
        public SubmitCategoryWithParentInValidator()
        {
            RuleFor(category => category.Name)
                .NotEmpty().WithMessage("Category name is required");
        }
    }
}
