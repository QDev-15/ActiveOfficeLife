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

            // Slug
            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required.")
                .Matches("^[a-z0-9-]+$").WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.")
                .Length(3, 100).WithMessage("Slug must be between 3 and 100 characters.");        

        }
    }
}
