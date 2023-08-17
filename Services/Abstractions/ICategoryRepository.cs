using SeeSay.Models.Dto.Categories;
using SeeSay.Models.Entities;

namespace SeeSay.Services.Abstractions;

public interface ICategoryRepository : IRepository<Category>
{
    public Task<ICollection<LightweightCategoryDto>> GetMostPopularLightweightCategoriesAsync(int limit = 20);
    public Task<ICollection<LightweightCategoryDto>> GetCategoriesAsync(string search, int limit = 40);
}