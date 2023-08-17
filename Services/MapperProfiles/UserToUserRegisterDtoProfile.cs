using AutoMapper;
using SeeSay.Models.Dto.Users;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class UserToUserRegisterDtoProfile : Profile
{
    public UserToUserRegisterDtoProfile()
    {
        CreateMap<User, UserRegisterDto>()
            .ReverseMap();
    }
}