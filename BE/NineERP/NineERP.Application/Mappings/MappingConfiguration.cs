using AutoMapper;
using NineERP.Application.Dtos.Role;
using NineERP.Application.Dtos.User;
using NineERP.Domain.Entities.Identity;
using NineERP.Application.Helpers;
using NineERP.Domain.Entities;
using NineERP.Application.Dtos.Department;

namespace NineERP.Application.Mappings
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration()
        {
            #region System
            CreateMap<DateTime, string>().ConvertUsing<DateTimeToStringConverter>();
            CreateMap<string, DateTime>().ConvertUsing<StringToDateTimeConverter>();
            #endregion

            #region Identity
            CreateMap<AppUser, UserDetailDto>().ReverseMap();
            CreateMap<AppRole, RoleDetailDto>().ReverseMap();
            CreateMap<AppRole, RoleResponse>().ReverseMap();
            #endregion

            #region Entity
            CreateMap<Department, DepartmentDto>().ReverseMap();
            #endregion
        }
    }
}
