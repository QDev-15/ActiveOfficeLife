using ActiveOfficeLife.Common.Requests;
using FluentValidation;

namespace ActiveOfficeLife.Admin.FluentValidations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
                .Length(4, 50).WithMessage("Tên đăng nhập phải từ 4 đến 50 ký tự");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .Length(6, 100).WithMessage("Mật khẩu phải từ 6 đến 100 ký tự");
            RuleFor(x => x.Remember)
                .NotNull().WithMessage("Vui lòng chọn tùy chọn ghi nhớ đăng nhập");
        }
    }
}
