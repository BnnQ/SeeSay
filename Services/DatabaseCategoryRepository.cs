using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SeeSay.Exceptions;
using SeeSay.Models.Contexts;
using SeeSay.Models.Dto.Categories;
using SeeSay.Models.Entities;
using SeeSay.Services.Abstractions;
using InvalidOperationException = System.InvalidOperationException;

namespace SeeSay.Services;

public class DatabaseCategoryRepository : ICategoryRepository
{
    private readonly SqlServerDbContext context;
    private readonly IMapper mapper;

    public DatabaseCategoryRepository(SqlServerDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<ICollection<LightweightCategoryDto>> GetMostPopularLightweightCategoriesAsync(
        int limit = 20)
    {
        if (context.Categories is null)
            throw new InvalidOperationException("Database set 'Categories' is null.");

        var categories =
            await context.Categories
                .Include(category => category.Posts)
                .OrderByDescending(category => category.Posts!.Sum(post => post.NumberOfViews))
                .Select(category => new LightweightCategoryDto(category.Id, category.Name))
                .Take(limit)
                .ToListAsync();

        return categories;
    }

    public async Task<ICollection<LightweightCategoryDto>> GetCategoriesAsync(string search, int limit = 40)
    {
        if (context.Categories is null)
            throw new InvalidOperationException("Database set 'Categories' is null.");

        var categories = await context.Categories.Where(category => category.Name.Contains(search))
            .OrderByDescending(category => category.Name.StartsWith(search))
            .Select(category => new LightweightCategoryDto(category.Id, category.Name))
            .ToListAsync();

        return categories;
    }

    public async Task<ICollection<Category>> GetAllAsync()
    {
        if (context.Categories is null)
            throw new InvalidOperationException("Database set 'Categories' is null.");

        var categories = await context.Categories.ToListAsync();
        return categories;
    }

    public async Task<Category> GetAsync(int id)
    {
        if (context.Categories is null)
            throw new InvalidOperationException("Database set 'Categories' is null.");

        var category = await context.Categories
            .Where(item => item.Id == id)
            .Include(item => item.Posts)
            .FirstAsync();

        if (category is null)
            throw new EntityNotFoundException($"Category with ID {id} not found.");

        return category;
    }

    public async Task AddAsync(Category entity)
    {
        if (context.Categories is null)
            throw new InvalidOperationException("Database set 'Categories' is null.");

        await context.Categories.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task EditAsync(int id, Category editedEntity)
    {
        if (context.Categories is null)
            throw new InvalidOperationException("Database set 'Categories' is null.");

        var category = await context.Categories.FindAsync(id);
        mapper.Map(source: editedEntity, destination: category);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        if (context.Categories is null)
            throw new InvalidOperationException("Database set 'Categories' is null.");
        
        var category = await context.Categories.FindAsync(id);
        if (category is null)
            return;

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }
    
}