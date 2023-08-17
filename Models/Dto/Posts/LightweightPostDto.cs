namespace SeeSay.Models.Dto.Posts;

public class LightweightPostDto
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public bool IsPremium { get; set; }

    public LightweightPostDto(int id, string description, string imagePath, bool isPremium)
    {
        Id = id;
        Description = description;
        ImagePath = imagePath;
        IsPremium = isPremium;
    }
    
}