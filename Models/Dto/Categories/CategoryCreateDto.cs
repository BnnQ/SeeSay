namespace SeeSay.Models.Dto.Categories;

public class CategoryCreateDto
{
    public string Name { get; set; } = null!;
    
    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(Name);
    }
    
}