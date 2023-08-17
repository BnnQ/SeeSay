namespace SeeSay.Models.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public int PostId { get; set; }
    public virtual Post Post { get; set; } = null!;
}