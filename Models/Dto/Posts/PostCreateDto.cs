using SeeSay.Models.Entities;

namespace SeeSay.Models.Dto.Posts;

public class PostCreateDto
{
    public string? Description { get; set; }
    public string SignalConnectionId { get; set; } = null!;
    public ICollection<Category> Categories { get; set; } = null!;
    public bool IsPremium { get; set; }
}