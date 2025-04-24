using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.Department;
using NineERP.Application.Features.DepartmentFeature.Commands;
using NineERP.Application.Features.DepartmentFeature.Queries;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Enums;

namespace NineERP.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DepartmentController(IMediator mediator, IValidator<DepartmentRequestDto> validator, ILocalizationService translate) : BaseController
    {
        [Authorize(Policy = $"Permission:{PermissionValue.Faculties.View}")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Faculties.Add}")]
        public async Task<IActionResult> Add([FromBody] DepartmentRequestDto request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                //var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return Json(new { succeeded = false, messages = translate["InvalidInput"] });
            }
            var result = await mediator.Send(new AddUpdateDepartmentCommand(request));
            if (!result.Succeeded) return Json(new { succeeded = false, messages = result.Messages.FirstOrDefault() });

            return Json(new { succeeded = true, messages = result.Messages.FirstOrDefault() });
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Faculties.Update}")]
        public async Task<IActionResult> Update([FromBody] DepartmentRequestDto request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                //var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return Json(new { succeeded = false, messages = translate["InvalidInput"] });
            }
            var result = await mediator.Send(new AddUpdateDepartmentCommand(request));
            if (!result.Succeeded) return Json(new { succeeded = false, messages = result.Messages.FirstOrDefault() });

            return Json(new { succeeded = true, messages = result.Messages.FirstOrDefault() });
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Faculties.View}")]
        public async Task<IActionResult> GetAllDepartmentPaging([FromQuery] DepartmentFilterDto request)
        {
            var result = await mediator.Send(new GetDepartmentsPaginationQuery(request));
            return Json(result);
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Faculties.View}")]
        public async Task<IActionResult> GetById(short id)
        {
            var result = await mediator.Send(new GetDepartmentByIdQuery(id));
            if (!result.Succeeded) return Json(new { succeeded = false, messages = result.Messages.FirstOrDefault() });

            return Json(new { succeeded = true, data = result.Data });
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Faculties.Delete}")]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await mediator.Send(new DeleteCommand(id));
            return Json(result);
        }
    }
}
