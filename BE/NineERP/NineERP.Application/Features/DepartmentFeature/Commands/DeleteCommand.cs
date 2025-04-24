using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.DepartmentFeature.Commands;

public record DeleteCommand(short Id) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, ILocalizationService translate) : IRequestHandler<DeleteCommand, IResult>
    {
        public async Task<IResult> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            var department = await context.Departments.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Id, cancellationToken);
            if (department == null) return await Result.FailAsync(translate["NotFound"]);

            department.IsDeleted = true;
            context.Departments.Update(department);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(translate["DeleteSuccess"]);
        }
    }
}