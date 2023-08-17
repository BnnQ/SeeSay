using SeeSay.Models.Entities;

namespace SeeSay.Services.Abstractions;

public interface ILikeRepository
{
    public Task<Like?> GetUserLikeAsync(string userId, int postId);
    public Task<int> GetNumberOfLikesAsync(int postId);
    public Task<int> GetNumberOfUserLikesAsync(string userId);
    public Task<IEnumerable<Like>> GetUserLikesAsync(string userId);

    public Task AddLikeAsync(Like like);
    public Task DeleteLikeAsync(string userId, int postId);
}