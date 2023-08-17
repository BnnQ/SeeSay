using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SeeSay.Models.Dto.Categories;
using SeeSay.Models.Entities;
using SeeSay.Services.Abstractions;

namespace SeeSay.Controllers;

[Route(template: "api/[controller]/[action]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository categoryRepository;
    private readonly IMapper mapper;

    public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
    {
        this.categoryRepository = categoryRepository;
        this.mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMostPopularCategories([FromQuery] int limit = 20)
    {
        return Ok(await categoryRepository.GetMostPopularLightweightCategoriesAsync(limit));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryCreateDto categoryCreateDto)
    {
        var category = mapper.Map<CategoryCreateDto, Category>(categoryCreateDto);
        await categoryRepository.AddAsync(category);
        return Created($"Post/GetPosts?categoryId={category.Id}", category);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategory([FromRoute] int id)
    {
        Category category;
        try
        {
            category = await categoryRepository.GetAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet("{search}")]
    public async Task<IActionResult> GetCategories([FromRoute] string search, [FromQuery] int limit)
    {
        var categories = await categoryRepository.GetCategoriesAsync(search, limit);
        return Ok(categories);
    }

}