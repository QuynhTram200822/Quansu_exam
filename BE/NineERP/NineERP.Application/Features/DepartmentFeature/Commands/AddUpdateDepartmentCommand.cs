using MediatR;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Department;
using NineERP.Domain.Entities;
using NineERP.Application.Interfaces.Common;

namespace NineERP.Application.Features.DepartmentFeature.Commands
{
    public record AddUpdateDepartmentCommand(DepartmentRequestDto Dto) : IRequest<IResult>
    {
        public class Handler(IApplicationDbContext context, ILocalizationService translate) : IRequestHandler<AddUpdateDepartmentCommand, IResult>
        {
            public async Task<IResult> Handle(AddUpdateDepartmentCommand request, CancellationToken cancellationToken)
            {
                // Add department
                if (request.Dto.Id == 0)
                {
                    // Check if slug already exists
                    var slugExists = await context.Departments.AsNoTracking().AnyAsync(x => x.Slug == request.Dto.Slug && !x.IsDeleted, cancellationToken);
                    if (slugExists) return await Result.FailAsync(translate["UrlAlreadyExists"]);
                    var add = new Department
                    {
                        Address1 = request.Dto.Address1,
                        Address2 = request.Dto.Address2,
                        BannerUrl = request.Dto.BannerUrl,
                        Description = request.Dto.Description,
                        Email = request.Dto.Email,
                        FaceBookUrl = request.Dto.FaceBookUrl,
                        LogoUrl = request.Dto.LogoUrl,
                        Name = request.Dto.Name,
                        PhoneNumber = request.Dto.PhoneNumber,
                        Slug = request.Dto.Slug,
                        WikipediaUrl = request.Dto.WikipediaUrl,
                        YoutubeUrl = request.Dto.YoutubeUrl
                    };

                    await context.Departments.AddAsync(add, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                    return await Result.SuccessAsync(translate["AddSuccess"]);
                }

                var slugUpdateExists = await context.Departments.AsNoTracking().AnyAsync(x => x.Id != request.Dto.Id && x.Slug == request.Dto.Slug && !x.IsDeleted, cancellationToken);
                if (slugUpdateExists) return await Result.FailAsync(translate["UrlAlreadyExists"]);

                // Update Department
                var department = await context.Departments.FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, cancellationToken);
                if (department == null) return await Result.FailAsync(translate["NotFound"]);

                department.Address1 = request.Dto.Address1;
                department.Address2 = request.Dto.Address2;
                department.BannerUrl = request.Dto.BannerUrl;
                department.Description = request.Dto.Description;
                department.Email = request.Dto.Email;
                department.FaceBookUrl = request.Dto.FaceBookUrl;
                department.LogoUrl = request.Dto.LogoUrl;
                department.Name = request.Dto.Name;
                department.PhoneNumber = request.Dto.PhoneNumber;
                department.Slug = request.Dto.Slug;
                department.WikipediaUrl = request.Dto.WikipediaUrl;
                department.YoutubeUrl = request.Dto.YoutubeUrl;

                context.Departments.Update(department);
                await context.SaveChangesAsync(cancellationToken);
                return await Result.SuccessAsync(translate["UpdateSuccess"]);
            }
        }
    }
}
