namespace SeeSay.Models.Entities;

public class Post
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public string ImagePath { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int NumberOfViews { get; set; }
    public int NumberOfDownloads { get; set; }
    public bool IsPremium { get; set; }
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Like>? Likes { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = null!;
    public virtual ICollection<Category> Categories { get; set; } = null!;
}