using NineERP.Domain.Entities;

namespace NineERP.Application.Interfaces.Common
{
    public interface IAuditLogService
    {
        Task AddAsync(AuditLog log);
    }
}
