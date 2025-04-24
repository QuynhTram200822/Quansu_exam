using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Department;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.DepartmentFeature.Queries;

public record GetDepartmentByIdQuery(short Id) : IRequest<Result<DepartmentDto>>
{
    public class Handler(IApplicationDbContext context, IMapper mapper, ILocalizationService translate) : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
    {
        public async Task<Result<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Departments.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Id, cancellationToken);
            var result = mapper.Map<DepartmentDto>(user);
            if (result == null) return await Result<DepartmentDto>.FailAsync(translate["NotFound"]);
            return await Result<DepartmentDto>.SuccessAsync(result);
        }
    }
}
