using SeeSay.Models.Entities;
using SeeSay.Models.Dto;

namespace SeeSay.Models.Dto.Users;

public class UserEditDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public ICollection<LinkEditDto>? SocialMediaLinks { get; set; }
}