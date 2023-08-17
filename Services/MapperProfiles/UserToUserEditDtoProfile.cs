using AutoMapper;
using SeeSay.Models.Dto.Users;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class UserToUserEditDtoProfile : Profile
{
    public UserToUserEditDtoProfile()
    {
        CreateMap<User, UserEditDto>()
            .ReverseMap();
    }
}