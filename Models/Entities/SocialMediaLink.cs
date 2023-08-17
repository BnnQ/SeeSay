namespace SeeSay.Models.Entities;

public class SocialMediaLink
{
    public int Id { get; set; }
    public string Link { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}