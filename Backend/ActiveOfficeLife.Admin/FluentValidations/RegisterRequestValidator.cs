using ActiveOfficeLife.Common.Requests;
using FluentValidation;

namespace ActiveOfficeLife.Admin.FluentValidations
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
                .Length(4, 50).WithMessage("Tên đăng nhập phải từ 4 đến 50 ký tự");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .Length(6, 100).WithMessage("Mật khẩu phải từ 6 đến 100 ký tự");
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống")
                .Equal(x => x.Password).WithMessage("Mật khẩu xác nhận không khớp với mật khẩu");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ");
        }
    }
}
