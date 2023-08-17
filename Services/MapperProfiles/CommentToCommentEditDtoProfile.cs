using AutoMapper;
using SeeSay.Models.Dto.Comments;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class CommentToCommentEditDtoProfile : Profile
{
    public CommentToCommentEditDtoProfile()
    {
        CreateMap<CommentEditDto, Comment>()
            .ReverseMap();
    }
}