using SeeSay.Models.Dto.Likes;

namespace SeeSay.Models.Dto.Posts;

public class FilterOptions
{
    public string? Search { get; set; }
    public ICollection<LikeDto>? Likes { get; set; }
    public string? UserId { get; set; }
    public int? CategoryId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 1;
}