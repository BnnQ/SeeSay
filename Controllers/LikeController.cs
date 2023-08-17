using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeeSay.Models.Dto.Likes;
using SeeSay.Models.Entities;
using SeeSay.Services.Abstractions;

namespace SeeSay.Controllers;

[Route(template: "api/[controller]/[action]")]
[ApiController]
[Authorize]
public class LikeController : ControllerBase
{
    private readonly ILikeRepository likeRepository;
    private readonly IMapper mapper;

    public LikeController(ILikeRepository likeRepository, IMapper mapper)
    {
        this.likeRepository = likeRepository;
        this.mapper = mapper;
    }

    [HttpGet("{userId}/{postId:int}")]
    public async Task<IActionResult> GetUserLikeAsync([FromRoute] string userId, [FromRoute] int postId)
    {
        var like = await likeRepository.GetUserLikeAsync(userId, postId);
        return Ok(new { IsUserLikePost = like is not null });
    }

    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetNumberOfLikesAsync([FromRoute] int postId)
    {
        var numberOfLikes = await likeRepository.GetNumberOfLikesAsync(postId);
        return Ok(numberOfLikes);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetNumberOfUserLikes([FromRoute] string userId)
    {
        var numberOfUserLikes = await likeRepository.GetNumberOfUserLikesAsync(userId);
        return Ok(numberOfUserLikes);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserLikes([FromRoute] string userId)
    {
        var likes = await likeRepository.GetUserLikesAsync(userId);
        return Ok(likes);
    }

    [HttpPost]
    public async Task<IActionResult> AddLike([FromBody] LikeDto likeDto)
    {
        var like = mapper.Map<LikeDto, Like>(likeDto);
        await likeRepository.AddLikeAsync(like);

        return Created($"Post/GetPost/{like.PostId}", like);
    }

    [HttpDelete("{userId}/{postId:int}")]
    public async Task<IActionResult> DeleteLike([FromRoute] string userId, [FromRoute] int postId)
    {
        await likeRepository.DeleteLikeAsync(userId, postId);
        return NoContent();
    }

}