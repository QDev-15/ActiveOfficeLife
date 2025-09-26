using ActiveOfficeLife.Common.Models;
using FluentValidation;

namespace ActiveOfficeLife.Admin.FluentValidations
{
    public class TagModelValidator  : AbstractValidator<TagModel>
    {
        public TagModelValidator()
        {
            RuleFor(tag => tag.Name)
                .NotEmpty().WithMessage("Tag Name không được trống.")
                .MaximumLength(100).WithMessage("Tag name phải không vượt quá 100 ký tự.");
        }
    }
}
