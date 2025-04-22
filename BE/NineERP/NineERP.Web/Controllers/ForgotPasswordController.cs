using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Entities.Identity;
using NineERP.Infrastructure.Contexts;
using NineERP.Infrastructure.Helpers;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NineERP.Web.Models;

namespace NineERP.Web.Controllers;

public class ForgotPasswordController(
    UserManager<AppUser> userManager,
    IEmailService emailService,
    ApplicationDbContext context,
    IAuditLogService auditLogService,
    IStringLocalizer<ForgotPasswordController> localizer
) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new ForgotPasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ForgotPasswordViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), localizer["EmailRequired"].Value);
        }
        else if (!new EmailAddressAttribute().IsValid(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), localizer["InvalidEmail"].Value);
        }

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            {
                Log.Warning("ForgotPassword - user not found or email unconfirmed: {Email}", model.Email);
                TempData["Message"] = localizer["CheckEmailInstructions"].Value;
                return RedirectToAction("Index", "Login");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "ForgotPassword", new { token, email = model.Email }, Request.Scheme);

            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var template = await context.EmailTemplates
                .Where(x => x.Code == "RESET_PASSWORD" && x.Language == culture && x.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (template is null)
            {
                Log.Warning("ForgotPassword - email template not found for culture {Culture}", culture);
                TempData["Message"] = localizer["EmailTemplateNotFound"].Value;
                return RedirectToAction("Index", "Login");
            }

            var subject = template.Subject;
            var body = template.Body
                .Replace("{{FullName}}", user.UserName ?? user.Email)
                .Replace("{{Email}}", user.Email)
                .Replace("{{ResetLink}}", resetLink);

            // Audit log
            var auditEntry = new AuditEntry(null!)
            {
                TableName = "Users",
                ActionType = "ForgotPassword"
            };
            auditEntry.KeyValues["UserId"] = user.Id;
            auditEntry.NewValues["Email"] = user.Email;

            var logEntry = auditEntry.ToAuditLog(user.Id, user.UserName ?? string.Empty, HttpContext.Connection.RemoteIpAddress?.ToString());
            await auditLogService.AddAsync(logEntry);

            Log.Information("ForgotPassword email requested for: {Email}", user.Email);

            // Gửi email async
            BackgroundJob.Enqueue(() => emailService.SendAsync(user.Email!, subject, body));

            TempData["Message"] = localizer["CheckEmailInstructions"].Value;
            return RedirectToAction("Index", "Login");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "ForgotPasswordController Index failed for email: {Email}", model.Email);
            ModelState.AddModelError(string.Empty, localizer["UnexpectedError"]?.Value ?? "Unexpected error occurred.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
        => View(new ResetPasswordViewModel { Token = token, Email = email });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                TempData["Message"] = localizer["PasswordResetFailed"].Value;
                return RedirectToAction("Index", "Login");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                Log.Information("Password reset successful for user: {Email}", user.Email);

                var auditEntry = new AuditEntry(null!)
                {
                    TableName = "Users",
                    ActionType = "ResetPassword"
                };
                auditEntry.KeyValues["UserId"] = user.Id;
                auditEntry.NewValues["Email"] = user.Email;

                var logEntry = auditEntry.ToAuditLog(user.Id, user.UserName ?? string.Empty, HttpContext.Connection.RemoteIpAddress?.ToString());
                await auditLogService.AddAsync(logEntry);

                TempData["Message"] = localizer["PasswordResetSuccess"].Value;
                return RedirectToAction("Index", "Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "ResetPassword failed for email: {Email}", model.Email);
            ModelState.AddModelError(string.Empty, localizer["UnexpectedError"]?.Value ?? "Unexpected error occurred.");
            return View(model);
        }
    }
}
