using FluentValidation;
using NineERP.Application.Dtos.Department;
using NineERP.Application.Constants.Messages;

namespace NineERP.Application.Validator
{
    public class DepartmentRequestDtoValidator : AbstractValidator<DepartmentRequestDto>
    {
        public DepartmentRequestDtoValidator()
        {
            RuleFor(x => x.Slug)
                .NotEmpty().WithErrorCode("DP001").WithMessage(ErrorMessages.GetMessage("DP001"))
                .MinimumLength(3).WithErrorCode("DP002").WithMessage(ErrorMessages.GetMessage("DP002"))
                .MaximumLength(100).WithErrorCode("DP003").WithMessage(ErrorMessages.GetMessage("DP003"))
                .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithErrorCode("DP004").WithMessage(ErrorMessages.GetMessage("DP004"));

            RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode("DP005").WithMessage(ErrorMessages.GetMessage("DP005"))
                .MinimumLength(2).WithErrorCode("DP006").WithMessage(ErrorMessages.GetMessage("DP006"))
                .MaximumLength(255).WithErrorCode("DP007").WithMessage(ErrorMessages.GetMessage("DP007"));

            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("DP008").WithMessage(ErrorMessages.GetMessage("DP008"))
                .EmailAddress().WithErrorCode("DP009").WithMessage(ErrorMessages.GetMessage("DP009"))
                .MaximumLength(255).WithErrorCode("DP010").WithMessage(ErrorMessages.GetMessage("DP010"));

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithErrorCode("DP011").WithMessage(ErrorMessages.GetMessage("DP011"))
                .Matches(@"^\d{9,15}$").WithErrorCode("DP012").WithMessage(ErrorMessages.GetMessage("DP012"));

            RuleFor(x => x.Address1)
                .NotEmpty().WithErrorCode("DP013").WithMessage(ErrorMessages.GetMessage("DP013"))
                .MinimumLength(5).WithErrorCode("DP014").WithMessage(ErrorMessages.GetMessage("DP014"))
                .MaximumLength(255).WithErrorCode("DP015").WithMessage(ErrorMessages.GetMessage("DP015"));

            RuleFor(x => x.Address2)
                .MaximumLength(255).WithErrorCode("DP016").WithMessage(ErrorMessages.GetMessage("DP016"));

            RuleFor(x => x.FaceBookUrl)
                .MaximumLength(255).WithErrorCode("DP017").WithMessage(ErrorMessages.GetMessage("DP017"))
                .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithErrorCode("DP018").WithMessage(ErrorMessages.GetMessage("DP018"));

            RuleFor(x => x.WikipediaUrl)
                .MaximumLength(255).WithErrorCode("DP019").WithMessage(ErrorMessages.GetMessage("DP019"))
                .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithErrorCode("DP020").WithMessage(ErrorMessages.GetMessage("DP020"));

            RuleFor(x => x.YoutubeUrl)
                .MaximumLength(255).WithErrorCode("DP021").WithMessage(ErrorMessages.GetMessage("DP021"))
                .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithErrorCode("DP022").WithMessage(ErrorMessages.GetMessage("DP022"));
        }
    }
}
