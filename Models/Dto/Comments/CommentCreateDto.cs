namespace SeeSay.Models.Dto.Comments;

public class CommentCreateDto
{
    public string Text { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public int PostId { get; set; }
    
    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(Text);
    }
}