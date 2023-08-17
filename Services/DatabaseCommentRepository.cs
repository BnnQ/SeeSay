using Microsoft.EntityFrameworkCore;
using SeeSay.Exceptions;
using SeeSay.Models.Contexts;
using SeeSay.Models.Entities;
using SeeSay.Services.Abstractions;

namespace SeeSay.Services;

public class DatabaseCommentRepository : ICommentRepository
{
    private readonly SqlServerDbContext context;

    public DatabaseCommentRepository(SqlServerDbContext context)
    {
        this.context = context;
    }
    
    public async Task<Comment> GetCommentAsync(int id)
    {
        if (context.Comments is null)
            throw new InvalidOperationException("Database set 'Comments' is null");
        
        var comment = await context.Comments.FindAsync(id);
        if (comment is null)
            throw new EntityNotFoundException($"Comment with ID {id} not found.");
        
        return comment;
    }
    
    public async Task<ICollection<Comment>> GetCommentsAsync(int postId)
    {
        if (context.Comments is null)
            throw new InvalidOperationException("Database set 'Comments' is null.");

        var comments = await context.Comments
            .Where(comment => comment.PostId == postId)
            .ToListAsync();
        
        return comments;
    }
    
    public async Task AddCommentAsync(Comment comment)
    {
        if (context.Comments is null)
            throw new InvalidOperationException("Database set 'Comments' is null.");

        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
    }
    
    public async Task<Comment> EditCommentAsync(int id, Comment editedComment)
    {
        if (context.Comments is null)
            throw new InvalidOperationException("Database set 'Comments' is null.");
        
        var comment = await context.Comments.FindAsync(id);
        comment!.Text = editedComment.Text;
        
        await context.SaveChangesAsync();
        return comment;
    }
    
    public async Task DeleteCommentAsync(int id)
    {
        if (context.Comments is null)
            throw new InvalidOperationException("Database set 'Comments' is null.");

        var comment = await context.Comments.FindAsync(id);
        if (comment is null)
            return;
        
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }
    
}