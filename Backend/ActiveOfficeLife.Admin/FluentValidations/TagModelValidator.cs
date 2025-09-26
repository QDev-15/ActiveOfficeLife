using ActiveOfficeLife.Common.Models;
using FluentValidation;

namespace ActiveOfficeLife.Admin.FluentValidations
{
    public class TagModelValidator  : AbstractValidator<TagModel>
    {
        public TagModelValidator()
        {
            RuleFor(tag => tag.Name)
                .NotEmpty().WithMessage("Tag name is required.")
                .MaximumLength(100).WithMessage("Tag name must not exceed 100 characters.");
        }
    }
}
