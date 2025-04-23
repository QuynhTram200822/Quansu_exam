using Microsoft.AspNetCore.Http;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Entities;
using NineERP.Infrastructure.Contexts;

namespace NineERP.Infrastructure.Services.Common
{
    public class AuditLogService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : IAuditLogService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task AddAsync(AuditLog log)
        {
            dbContext.AuditLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}
