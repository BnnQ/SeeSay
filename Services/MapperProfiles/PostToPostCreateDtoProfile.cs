using AutoMapper;
using SeeSay.Models.Dto.Posts;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class PostToPostCreateDtoProfile : Profile
{
    public PostToPostCreateDtoProfile()
    {
        CreateMap<Post, PostCreateDto>()
            .ReverseMap();
    }
}