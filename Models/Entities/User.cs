using Microsoft.AspNetCore.Identity;

namespace SeeSay.Models.Entities;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [PersonalData]
    public string? Location { get; set; }
    [PersonalData]
    public string? Bio { get; set; }

    public bool HasPremium { get; set; } = false;
    public string AvatarImagePath { get; set; } = null!;
    
    public virtual ICollection<SocialMediaLink>? SocialMediaLinks { get; set; }
    public virtual ICollection<Post>? Posts { get; set; }
    public virtual ICollection<Like>? Likes { get; set; }
    public virtual ICollection<Comment>? Comments { get; set; }
}