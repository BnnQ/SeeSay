namespace SeeSay.Models.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public virtual ICollection<Post>? Posts { get; set; }
}