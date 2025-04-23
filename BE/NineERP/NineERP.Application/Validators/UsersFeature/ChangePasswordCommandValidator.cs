using FluentValidation;
using NineERP.Application.Features.UsersFeature.Commands;
namespace NineERP.Application.Validators.UsersFeature
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Model.Password)
                .NotEmpty().WithMessage("Mật khẩu hiện tại không được để trống")
                .MinimumLength(6).WithMessage("Tối thiểu 6 ký tự");

            RuleFor(x => x.Model.NewPassword)
                .NotEmpty().WithMessage("Mật khẩu mới không được để trống")
                .MinimumLength(6).WithMessage("Tối thiểu 6 ký tự")
                .Matches("[A-Z]").WithMessage("Phải có ít nhất một chữ hoa")
                .Matches("[0-9]").WithMessage("Phải có ít nhất một chữ số");

            RuleFor(x => x.Model.ConfirmNewPassword)
                .Equal(x => x.Model.NewPassword).WithMessage("Mật khẩu xác nhận không khớp");
        }
    }
}