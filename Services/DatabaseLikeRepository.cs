using Microsoft.EntityFrameworkCore;
using SeeSay.Models.Contexts;
using SeeSay.Models.Entities;
using SeeSay.Services.Abstractions;

namespace SeeSay.Services;

public class DatabaseLikeRepository : ILikeRepository
{
    private readonly SqlServerDbContext context;

    public DatabaseLikeRepository(SqlServerDbContext context)
    {
        this.context = context;
    }
    
    public async Task<Like?> GetUserLikeAsync(string userId, int postId)
    {
        if (context.Likes is null)
            throw new InvalidOperationException("Database set 'Likes' is null.");
        
        var like =
            await context.Likes
                .Where(like => like.UserId.Equals(userId) && like.PostId == postId)
                .FirstOrDefaultAsync();

        return like;
    }
    
    public async Task<int> GetNumberOfLikesAsync(int postId)
    {
        if (context.Likes is null)
            throw new InvalidOperationException("Database set 'Likes' is null.");

        var numberOfLikes = await context.Likes.Where(like => like.PostId == postId).CountAsync();
        return numberOfLikes;
    }
    
    public async Task<int> GetNumberOfUserLikesAsync(string userId)
    {
        if (context.Likes is null)
            throw new InvalidOperationException("Database set 'Likes' is null.");

        var numberOfLikes = await context.Likes.Where(like => like.UserId.Equals(userId)).CountAsync();
        return numberOfLikes;
    }
    
    public async Task<IEnumerable<Like>> GetUserLikesAsync(string userId)
    {
        if (context.Likes is null)
            throw new InvalidOperationException("Database set 'Likes' is null.");

        var likes = await context.Likes.Where(like => like.UserId.Equals(userId))
            .ToListAsync();

        return likes;
    }
    
    public async Task AddLikeAsync(Like like)
    {
        if (context.Likes is null)
            throw new InvalidOperationException("Database set 'Likes' is null.");

        await context.AddAsync(like);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteLikeAsync(string userId, int postId)
    {
        if (context.Likes is null)
            throw new InvalidOperationException("Database set 'Likes' is null.");

        var like = await context.Likes
            .Where(like => like.UserId.Equals(userId) && like.PostId == postId)
            .FirstOrDefaultAsync();
        
        if (like is null)
            return;
        
        context.Likes.Remove(like);
        await context.SaveChangesAsync();
    }
}