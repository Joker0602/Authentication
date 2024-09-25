using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using URAuth.Dtos.RoleDto;
using URAuth.Dtos.UserDto;
using URAuth.Dtos.UserRoleDto;
using URAuth.Model;

namespace URAuth.Profiles
{
    public class SettingProfiles:Profile
    {
        public SettingProfiles(){

            //user
            CreateMap<User, UserRegisterDto>().ReverseMap();
            CreateMap<User, UserApprovalReadDto>().ReverseMap();
            CreateMap<User, UserChangePasswordDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UserReadDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();

            //role
            CreateMap<Role, RoleReadDto>().ReverseMap();

            //userrole
            CreateMap<UserRole,UserRoleCreateDto>().ReverseMap();
            CreateMap<UserRole,UserRoleReadDto>().ReverseMap();
            CreateMap<UserRole,UserRoleUpdateDto>().ReverseMap();

        }
    }
}