using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Entities.Identity;
using NineERP.Infrastructure.Helpers;
using NineERP.Web.Models.Login;
using Serilog;
using UAParser;

namespace NineERP.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IAuditLogService auditLogService,
        IStringLocalizer<LoginController> localizer)
        : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (authResult.Succeeded && authResult.Principal != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(LoginViewModel model, string? returnUrl = null)
        {
            var user = await userManager.FindByNameAsync(model.UserName);

            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                Log.Warning("Login failed for user: {User}", model.UserName);
                return Json(new { succeeded = false, messages = localizer["UserNameOrPasswordInvalid"] });
            }

            if (user.LockoutEnabled)
            {
                Log.Warning("Login blocked for locked user: {User}", model.UserName);
                return Json(new { succeeded = false, messages = localizer["LockUser"] });
            }

            var roleNames = await userManager.GetRolesAsync(user);

            var permissionClaims = new List<Claim>();
            foreach (var roleName in roleNames)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claimsList = await roleManager.GetClaimsAsync(role);
                    permissionClaims.AddRange(claimsList.Where(c => c.Type == "permission"));
                }
            }

            if (!permissionClaims.Any())
            {
                Log.Warning("Login rejected for user without permission: {User}", model.UserName);
                return Json(new { succeeded = false, messages = localizer["AccountAccess"] });
            }

            var securityStamp = await userManager.GetSecurityStampAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("FullName", user.FullName),
                new("Avatar", user.AvatarUrl ?? string.Empty),
                new("AspNet.Identity.SecurityStamp", securityStamp)
            };

            foreach (var role in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            claims.AddRange(permissionClaims);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(14)   // nếu tick Remember Me
                        : DateTimeOffset.UtcNow.AddHours(2)   // nếu không
                });

            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName ?? "");
            HttpContext.Session.SetString("FullName", user.FullName);

            Log.Information("User logged in: {User}", user.UserName);
            var userAgent = Request.Headers["User-Agent"].ToString();
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);
            
            var auditEntry = new AuditEntry(null!)
            {
                TableName = "Users",
                ActionType = "Login",
                KeyValues =
                {
                    ["UserId"] = user.Id
                },
                NewValues =
                {
                    ["UserName"] = user.UserName,
                    ["FullName"] = user.FullName,
                    ["IsPersistent"] = model.RememberMe.ToString(),
                    ["UserAgent"] = userAgent,
                    ["Browser"] = clientInfo.UA.ToString(), // ví dụ: Chrome 120.0
                    ["OS"] = clientInfo.OS.ToString(),       // ví dụ: Windows 10
                    ["Device"] = clientInfo.Device.ToString()
                }
            };

            var logEntry = auditEntry.ToAuditLog(user.Id, user.UserName ?? string.Empty, HttpContext.Connection.RemoteIpAddress?.ToString());
            await auditLogService.AddAsync(logEntry);

            return Json(new
            {
                succeeded = true,
                redirectUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Dashboard")
            });
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
