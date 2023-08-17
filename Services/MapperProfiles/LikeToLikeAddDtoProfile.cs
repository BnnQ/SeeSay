using AutoMapper;
using SeeSay.Models.Dto.Likes;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class LikeToLikeAddDtoProfile : Profile
{
    public LikeToLikeAddDtoProfile()
    {
        CreateMap<Like, LikeDto>()
            .ReverseMap();
    }
}