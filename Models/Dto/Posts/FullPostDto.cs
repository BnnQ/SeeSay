using SeeSay.Models.Entities;

namespace SeeSay.Models.Dto.Posts;

public class FullPostDto
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public string ImagePath { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int NumberOfViews {get; set; }
    public int NumberOfDownloads { get; set; }
    public int NumberOfLikes { get; set; }
    public bool IsPremium { get; set; }
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Comment>? Comments { get; set; }
    public virtual ICollection<Category> Categories { get; set; } = null!;
}