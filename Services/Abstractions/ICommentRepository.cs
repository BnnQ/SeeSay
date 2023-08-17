using SeeSay.Models.Entities;

namespace SeeSay.Services.Abstractions;

public interface ICommentRepository
{
    public Task<Comment> GetCommentAsync(int id);
    public Task<ICollection<Comment>> GetCommentsAsync(int postId);
    public Task AddCommentAsync(Comment comment);
    public Task<Comment> EditCommentAsync(int id, Comment editedComment);
    public Task DeleteCommentAsync(int id);
}