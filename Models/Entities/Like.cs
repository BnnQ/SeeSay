namespace SeeSay.Models.Entities;

public class Like
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public int PostId { get; set; }
    public virtual Post Post { get; set; } = null!;
}