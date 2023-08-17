using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SeeSay.Exceptions;
using SeeSay.Models.Contexts;
using SeeSay.Models.Dto.Likes;
using SeeSay.Models.Dto.Posts;
using SeeSay.Models.Entities;
using SeeSay.Services.Abstractions;
using System.Text.Json;
using InvalidOperationException = System.InvalidOperationException;

namespace SeeSay.Services;

public class DatabasePostRepository : IPostRepository
{
    private readonly SqlServerDbContext context;
    private readonly IMapper mapper;

    public DatabasePostRepository(SqlServerDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<ICollection<Post>> GetAllAsync()
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var posts = await context.Posts.ToListAsync();
        return posts;
    }

    public async Task<FullPostDto> GetAsync(int id)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var post = await context.Posts
            .Where(item => item.Id == id)
            .Include(item => item.Categories)
            .Include(item => item.Comments)
            .ThenInclude(comment => comment.User)
            .Include(item => item.User)
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (post is null)
            throw new EntityNotFoundException($"Post with ID {id} not found.");

        if (context.Likes is null)
            throw new InvalidOperationException("'Likes' database set is null.");

        var numberOfLikes = await context.Likes.Where(like => like.PostId == id)
            .CountAsync();

        post.NumberOfViews += 1;
        await context.SaveChangesAsync();

        var response = mapper.Map<Post, FullPostDto>(post);
        response.NumberOfLikes = numberOfLikes;
        return response;
    }

    public async Task AddAsync(Post entity)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        if (context.Categories is null)
            throw new InvalidOperationException("Database set 'Categories' is null.");
        
        var categories = new List<Category>();
        for (var i = 0; i < entity.Categories.Count; i++)
        {
            var category = entity.Categories.ElementAt(i);
            Console.WriteLine(JsonSerializer.Serialize(entity.Categories));
            if (category.Id != 0)
            {
                var existingCategory = await context.Categories.FindAsync(category.Id);
                if (existingCategory != null)
                {
                    categories.Add(existingCategory);
                }
            }
            else
            {
                categories.Add(category);  
            }
        }

        entity.Categories = categories;
        await context.Posts.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task EditAsync(int id, Post editedEntity)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var post = await context.Posts.FindAsync(id);
        mapper.Map(editedEntity, post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var post = await context.Posts.FindAsync(id);
        if (post is null)
            return;

        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LightweightPostDto>> GetLightweightPostsAsync(string? search = null,
        int page = 1,
        int pageSize = 21)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        IQueryable<Post> query = context.Posts;
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(post => post.Description.Contains(search) ||
                                        post.Categories.Any(category => category.Name.Contains(search)));
        }

        var posts = await query
            .OrderByDescending(post => post.NumberOfViews + post.NumberOfDownloads * 3)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(post =>
                new LightweightPostDto(post.Id, post.Description, post.ImagePath, post.IsPremium))
            .ToListAsync();

        return posts;
    }

    public async Task<IEnumerable<LightweightPostDto>> GetLightweightUserPostsAsync(string userId,
        int page = 1,
        int pageSize = 21)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var posts = await context.Posts
            .Where(post => post.UserId.Equals(userId))
            .OrderByDescending(post => post.NumberOfViews + post.NumberOfDownloads * 3)
            .Select(post =>
                new LightweightPostDto(post.Id, post.Description, post.ImagePath, post.IsPremium))
            .ToListAsync();

        return posts;
    }

    public async Task<IEnumerable<LightweightPostDto>> GetLightweightLikedPostsAsync(
        IEnumerable<LikeDto> likes,
        int page = 1,
        int pageSize = 21)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var likeDtos = likes as LikeDto[] ?? likes.ToArray();
        var postIds = likeDtos.Select(l => l.PostId)
            .ToList();
        var userIds = likeDtos.Select(l => l.UserId)
            .ToList();

        var posts = await context.Posts
            .Where(post =>
                post.Likes != null &&
                post.Likes.Any(like => postIds.Contains(like.PostId) && userIds.Contains(like.UserId)))
            .OrderByDescending(post => post.NumberOfViews + post.NumberOfDownloads * 3)
            .Select(post =>
                new LightweightPostDto(post.Id, post.Description, post.ImagePath, post.IsPremium))
            .ToListAsync();

        return posts;
    }


    public async Task<IEnumerable<LightweightPostDto>> GetLightweightCategoryPostsAsync(int categoryId,
        int page = 1,
        int pageSize = 21)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var posts = await context.Posts
            .Include(post => post.Categories)
            .Where(post => post.Categories.Any(category => category.Id == categoryId))
            .OrderByDescending(post => post.NumberOfViews + post.NumberOfDownloads * 3)
            .Select(post =>
                new LightweightPostDto(post.Id, post.Description, post.ImagePath, post.IsPremium))
            .ToListAsync();

        return posts;
    }

    public async Task<int> GetNumberOfPostPages(int pageSize = 21)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var numberOfPosts = await context.Posts.CountAsync();
        var numberOfPages = (int)Math.Ceiling((double)numberOfPosts / pageSize);

        return numberOfPages;
    }

    public async Task<int> GetNumberOfUserPosts(string userId)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var numberOfPosts = await context.Posts.Where(post => post.UserId.Equals(userId))
            .CountAsync();

        return numberOfPosts;
    }

    public async Task IncrementNumberOfDownloads(int id)
    {
        if (context.Posts is null)
            throw new InvalidOperationException("Database set 'Posts' is null.");

        var post = await context.Posts.FindAsync(id);
        if (post is null)
            throw new EntityNotFoundException();

        post.NumberOfDownloads += 1;
        await context.SaveChangesAsync();
    }
    
}