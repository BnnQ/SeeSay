using SeeSay.Models.Dto.Likes;
using SeeSay.Models.Dto.Posts;
using SeeSay.Models.Entities;

namespace SeeSay.Services.Abstractions;

public interface IPostRepository
{
    public Task<FullPostDto> GetAsync(int id);
    public Task AddAsync(Post entity);
    public Task EditAsync(int id, Post editedEntity);
    public Task DeleteAsync(int id);
    
    
    public Task<IEnumerable<LightweightPostDto>> GetLightweightPostsAsync(string? search = null, int page = 1, int pageSize = 21);

    public Task<IEnumerable<LightweightPostDto>> GetLightweightUserPostsAsync(string userId,
        int page = 1,
        int pageSize = 21);

    public Task<IEnumerable<LightweightPostDto>> GetLightweightLikedPostsAsync(IEnumerable<LikeDto> likes, int page = 1, int pageSize = 21);

    public Task<IEnumerable<LightweightPostDto>> GetLightweightCategoryPostsAsync(int categoryId, int page = 1, int pageSize = 21);
    public Task<int> GetNumberOfPostPages(int pageSize = 21);
    public Task<int> GetNumberOfUserPosts(string userId);

    public Task IncrementNumberOfDownloads(int id);
}