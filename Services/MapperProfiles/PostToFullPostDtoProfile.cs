using AutoMapper;
using SeeSay.Models.Dto.Posts;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class PostToFullPostDtoProfile : Profile
{
    public PostToFullPostDtoProfile()
    {
        CreateMap<Post, FullPostDto>();
    }
}