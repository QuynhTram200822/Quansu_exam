using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Domain.Entities;
using NineERP.Domain.Entities.Base;
using NineERP.Domain.Entities.Identity;
using NineERP.Infrastructure.Helpers;

namespace NineERP.Infrastructure.Contexts
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
        : IdentityDbContext<AppUser, AppRole, string,
            IdentityUserClaim<string>, IdentityUserRole<string>,
            IdentityUserLogin<string>, AppRoleClaim, IdentityUserToken<string>>(options),
          IApplicationDbContext
    {
        #region DbSets

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<CronJob> CronJobs => Set<CronJob>();
        public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
        public DbSet<GeneralSetting> GeneralSettings => Set<GeneralSetting>();
        public DbSet<EmailSetting> EmailSettings => Set<EmailSetting>();

        #endregion

        #region SaveChanges + Audit
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Database.BeginTransactionAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Handle audit timestamps
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = dateTimeService.Now;
                        entry.Entity.CreatedBy = currentUserService.UserName ?? "SuperAdmin";
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = dateTimeService.Now;
                        entry.Entity.LastModifiedBy = currentUserService.UserName ?? "SuperAdmin";
                        break;
                }
            }

            // Build Audit Logs
            var auditLogs = new List<AuditLog>();

            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                            e.State != EntityState.Detached &&
                            e.State != EntityState.Unchanged))
            {
                var audit = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    ActionType = entry.State.ToString()
                };

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    if (property.Metadata.IsPrimaryKey())
                    {
                        audit.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            audit.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            audit.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                audit.OldValues[propertyName] = property.OriginalValue;
                                audit.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }

                auditLogs.Add(audit.ToAuditLog(
                    currentUserService.UserId ?? "SYSTEM",
                    currentUserService.UserName ?? "SYSTEM",
                    currentUserService.Origin
                ));
            }

            if (auditLogs.Any())
                AuditLogs.AddRange(auditLogs);

            return await base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Identity Model Config

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable("Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                entity.HasIndex(e => e.NormalizedEmail).HasDatabaseName("EmailIndex");
            });

            builder.Entity<AppRole>().ToTable("Roles", "Identity");

            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Identity");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Identity");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Identity");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Identity");

            builder.Entity<AppRoleClaim>(entity =>
            {
                entity.ToTable("RoleClaims", "Identity");
                entity.HasOne(rc => rc.Role)
                      .WithMany(r => r.RoleClaims)
                      .HasForeignKey(rc => rc.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        #endregion
    }
}
