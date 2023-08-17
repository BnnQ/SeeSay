using AutoMapper;
using SeeSay.Models.Dto.Users;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class UserToUserLoginDtoProfile : Profile
{
    public UserToUserLoginDtoProfile()
    {
        CreateMap<User, UserLoginDto>()
            .ReverseMap();
    }
}