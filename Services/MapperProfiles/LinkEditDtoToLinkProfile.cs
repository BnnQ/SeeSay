using AutoMapper;
using SeeSay.Models.Dto;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class LinkEditDtoToLinkProfile : Profile
{
    public LinkEditDtoToLinkProfile()
    {
        CreateMap<LinkEditDto, SocialMediaLink>();
    }
}