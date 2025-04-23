using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Department;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.DepartmentFeature.Queries;

public record GetDepartmentsPaginationQuery(DepartmentFilterDto Dto) : IRequest<PaginatedResult<DepartmentResponsePaginationDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetDepartmentsPaginationQuery, PaginatedResult<DepartmentResponsePaginationDto>>
    {
        public async Task<PaginatedResult<DepartmentResponsePaginationDto>> Handle(GetDepartmentsPaginationQuery request, CancellationToken cancellationToken)
        {
            var keyword = request.Dto.Keyword?.ToLower();

            var query = from d in context.Departments.AsNoTracking()
                        where !d.IsDeleted
                           && (string.IsNullOrEmpty(keyword)
                            || EF.Functions.Like(d.Name, $"%{keyword}%")
                            || EF.Functions.Like(d.Slug, $"%{keyword}%"))
                        select new DepartmentResponsePaginationDto
                        {
                            Name = d.Name,
                            Slug = d.Slug,
                            PhoneNumber = d.PhoneNumber,
                            Email = d.Email,
                            Id = d.Id
                        };

            var totalRecords = await query.CountAsync(cancellationToken);

            var result = await query.OrderByDescending(x => x.Id)
                                    .Skip((request.Dto.PageNumber - 1) * request.Dto.PageSize)
                                    .Take(request.Dto.PageSize)
                                    .ToListAsync(cancellationToken);

            return PaginatedResult<DepartmentResponsePaginationDto>.Success(result, totalRecords, request.Dto.PageNumber, request.Dto.PageSize);
        }
    }
}
