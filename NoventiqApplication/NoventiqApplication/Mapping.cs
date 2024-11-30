using AutoMapper;


namespace NoventiqApplication
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User -> UserDTO
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.RoleId).ToList()));

            // UserDTO -> User
            CreateMap<UserDTO, User>();

            // Role -> RoleDTO
            CreateMap<Role, RoleDTO>();

            // RoleDTO -> Role
            CreateMap<RoleDTO, Role>();
        }
    }

}
