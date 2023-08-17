using AutoMapper;
using SeeSay.Models.Dto.Comments;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class CommentToCommentCreateDtoProfile : Profile
{
    public CommentToCommentCreateDtoProfile()
    {
        CreateMap<Comment, CommentCreateDto>()
            .ReverseMap();
    }
}