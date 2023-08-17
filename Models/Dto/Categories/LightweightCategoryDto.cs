namespace SeeSay.Models.Dto.Categories;

public class LightweightCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public LightweightCategoryDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
    
}