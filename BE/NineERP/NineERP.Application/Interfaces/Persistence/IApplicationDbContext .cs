using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NineERP.Domain.Entities;
using NineERP.Domain.Entities.Identity;

namespace NineERP.Application.Interfaces.Persistence
{
    public interface IApplicationDbContext
    {
        // Identity
        DbSet<AppUser> Users { get; }
        DbSet<AppRole> Roles { get; }
        DbSet<AppRoleClaim> RoleClaims { get; }
        DbSet<IdentityUserRole<string>> UserRoles { get; }
        DbSet<AuditLog> AuditLogs { get; }
        // Entities
        DbSet<EmailTemplate> EmailTemplates { get; }
        DbSet<GeneralSetting> GeneralSettings { get; }
        DbSet<EmailSetting> EmailSettings { get; }
        DbSet<Department> Departments { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
