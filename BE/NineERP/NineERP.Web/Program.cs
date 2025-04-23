using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NineERP.Application.Configurations;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Identity;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Mappings;
using NineERP.Domain.Entities.Identity;
using NineERP.Infrastructure.Authorization;
using NineERP.Infrastructure.Contexts;
using NineERP.Infrastructure.Services.Common;
using NineERP.Infrastructure.Services.Identity;
using NineERP.Infrastructure.Services.SecurityStamp;
using reCAPTCHA.AspNetCore;
using Serilog;
using System.Globalization;
using FluentValidation;
using NineERP.Application.Dtos.Department;
using NineERP.Application.Validator;
using NineERP.Web.Authorization;
using NineERP.Web.Services;
using CurrentUserService = NineERP.Infrastructure.Services.Identity.CurrentUserService; // 🧠 QUAN TRỌNG

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// 🔠 Localization setup
services.AddLocalization(options => options.ResourcesPath = "Resources");

services.Configure<RequestLocalizationOptions>(opts =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("vi-VN"),
        new CultureInfo("en-US")
    };

    opts.DefaultRequestCulture = new RequestCulture("vi-VN");
    opts.SupportedCultures = supportedCultures;
    opts.SupportedUICultures = supportedCultures;
    opts.RequestCultureProviders = new IRequestCultureProvider[] { new CookieRequestCultureProvider() };
});

// 🧩 Razor + localization
services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization()
    .AddRazorRuntimeCompilation();

services.AddRazorPages();

// 🍪 Cookie policy
services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// 🔐 reCAPTCHA
services.AddRecaptcha(options => configuration.GetSection("Recaptcha").Bind(options));

// 🗃️ EF Core + Identity
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        .EnableSensitiveDataLogging());

services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("Session:DefaultLockoutTimeSpan");
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("Session:DefaultLockoutTimeSpan"));
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ✅ Cookie config override
services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// 🧾 Cookie auth
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

// 🛡️ Authorization
services.AddAuthorization();
services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// 🕒 Hangfire
services.AddHangfire(x =>
{
    x.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
     .UseSimpleAssemblyNameTypeSerializer()
     .UseRecommendedSerializerSettings()
     .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
     {
         CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
         SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
         QueuePollInterval = TimeSpan.FromSeconds(15),
         UseRecommendedIsolationLevel = true,
         DisableGlobalLocks = true
     });
});
services.AddHangfireServer();

// ⚙️ Add MediatR
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining(typeof(NineERP.Application.AssemblyReference))); // 🔥 THÊM DÒNG NÀY

// 🔄 Others
services.AddAutoMapper(typeof(MappingConfiguration).Assembly);
services.AddMemoryCache();
services.AddHttpContextAccessor();
// ✅ CẤU HÌNH MAIL
services.Configure<MailConfiguration>(
    configuration.GetSection("MailConfiguration"));

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(configuration.GetValue<int>("Session:IdleTimeoutHours"));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 📦 App services
services.AddScoped<ILocalizationService, LocalizationService>();
services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
services.AddScoped<ICurrentUserService, CurrentUserService>();
services.AddScoped<CurrentUserService>();
services.AddScoped<IDateTimeService, DateTimeService>();
services.AddScoped<ITokenService, IdentityService>();
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<IValidator<DepartmentRequestDto>, DepartmentRequestDtoValidator>();
services.AddScoped<IAuditLogService, AuditLogService>();
services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
services.AddScoped<SecurityStampCacheService>();

// 📄 Logging
builder.Host.UseSerilog((ctx, config) =>
{
    config.ReadFrom.Configuration(ctx.Configuration);
});

var app = builder.Build();

// 🧪 Seed database
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    seeder.Initialize();
}

// 🧱 Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Exception");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

app.UseStaticFiles();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseRouting();
app.UseCookiePolicy();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorization() }
});

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Login}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "DepartmentDefault",
    pattern: "khoa/{slug}",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "DepartmentModule",
    pattern: "khoa/{slug}/{controller}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
