using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeeSay.Exceptions;
using SeeSay.Models.Dto.Comments;
using SeeSay.Models.Entities;
using SeeSay.Services.Abstractions;
using SeeSay.Utils.Extensions;

namespace SeeSay.Controllers;

[Route(template: "api/[controller]/[action]")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository commentRepository;
    private readonly IMapper mapper;
    private readonly UserManager<User> userManager;

    public CommentController(ICommentRepository commentRepository, UserManager<User> userManager, IMapper mapper)
    {
        this.commentRepository = commentRepository;
        this.userManager = userManager;
        this.mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateDto commentCreateDto)
    {
        var comment = mapper.Map<CommentCreateDto, Comment>(commentCreateDto);
        await commentRepository.AddCommentAsync(comment);
        var user = await userManager.FindByIdAsync(comment.UserId);
        if (user is null)
            throw new EntityNotFoundException();
        
        comment.User = user;
        return Created($"Post/GetPost/{comment.PostId}", comment);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> EditComment([FromRoute] int id, [FromBody] CommentEditDto commentEditDto)
    {
        var comment = await commentRepository.GetCommentAsync(id);
        
        if (comment.UserId != User.GetCurrentUserId())
            return Forbid();
        
        
        comment = await commentRepository.EditCommentAsync(id, mapper.Map<CommentEditDto, Comment>(commentEditDto));
        return Ok(comment);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        var comment = await commentRepository.GetCommentAsync(id);
        if (comment.UserId != User.GetCurrentUserId() && !User.IsInRole("Moderator"))
            return Forbid();

        await commentRepository.DeleteCommentAsync(id);
        return NoContent();
    }

}