using ActiveOfficeLife.Common.Models;
using FluentValidation;

namespace ActiveOfficeLife.Admin.FluentValidations
{
    public class PostModelValidator  : AbstractValidator<PostModel>
    {
        public PostModelValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống")
                .MaximumLength(200).WithMessage("Tiêu đề không được vượt quá 200 ký tự");
            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug không được để trống")
                .MaximumLength(200).WithMessage("Slug không được vượt quá 200 ký tự");
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Vui lòng chọn danh mục");
            // khuyến nghị dùng TagIds để bind checkbox
            RuleFor(x => x.TagIds)
                .Must(ids => ids == null || ids.Count <= 10)
                .WithMessage("Không được chọn quá 10 thẻ");
        }
    }
}
