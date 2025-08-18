using ActiveOfficeLife.Common.Models;
using FluentValidation;

namespace ActiveOfficeLife.Admin.FluentValidations
{
    public class CategoryModelValidator : AbstractValidator<CategoryModel>
    {
        public CategoryModelValidator()
        {
            // Name
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .Length(3, 100).WithMessage("Category name must be between 3 and 100 characters.");
        }
    }
}
