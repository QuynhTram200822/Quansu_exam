using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Constants.Role;
using NineERP.Application.Dtos.EmailSetting;
using NineERP.Application.Features.EmailSettingsFeature.Commands;
using NineERP.Application.Features.EmailSettingsFeature.Queries;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Enums;
using NineERP.Infrastructure.Helpers;

namespace NineERP.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class EmailSettingsController(
    IMediator mediator,
    IAuditLogService auditLogService,
    ILogger<EmailSettingsController> logger,
    ICurrentUserService currentUser
) : Controller
{
    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.EmailSettings.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("EmailSettings/GetData")]
    [Authorize(Policy = $"Permission:{PermissionValue.EmailSettings.View}")]
    public async Task<IActionResult> GetData()
    {
        var result = await mediator.Send(new GetEmailSettingsQuery());
        return Ok(result.Data); // Trả về dữ liệu thuần để JS bind vào form
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.EmailSettings.Update}")]
    public async Task<IActionResult> SaveEmailSettings([FromForm] EmailSettingRequest request)
    {
        try
        {
            var result = await mediator.Send(new UpdateEmailSettingsCommand(request));

            // Ghi audit log nếu thành công
            if (result.Succeeded)
            {
                var audit = new AuditEntry(null!)
                {
                    TableName = "EmailSettings",
                    ActionType = "Update"
                };

                audit.NewValues["Protocol"] = request.Protocol.ToString();
                audit.NewValues["SenderEmail"] = request.SenderEmail;
                audit.NewValues["SenderName"] = request.SenderName;

                var log = audit.ToAuditLog(
                    currentUser.UserId ?? "SYSTEM",
                    currentUser.UserName ?? "SYSTEM",
                    currentUser.Origin
                );
                await auditLogService.AddAsync(log);
            }

            return Json(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi lưu EmailSettings");
            return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu cấu hình Email." });
        }
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.EmailSettings.View}")]
    public async Task<IActionResult> SendTestEmail([FromForm] string testEmail)
    {
        try
        {
            var result = await mediator.Send(new SendTestEmailCommand(testEmail));

            // Ghi audit log
            var audit = new AuditEntry(null!)
            {
                TableName = "EmailSettings",
                ActionType = "SendTestEmail"
            };
            audit.NewValues["TestEmail"] = testEmail;

            var log = audit.ToAuditLog(
                currentUser.UserId ?? RoleConstants.SuperAdmin,
                currentUser.UserName ?? RoleConstants.SuperAdmin,
                currentUser.Origin
            );
            await auditLogService.AddAsync(log);

            return Json(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi gửi test email đến {Email}", testEmail);
            return Json(new { success = false, message = "Gửi email kiểm tra thất bại." });
        }
    }
}
