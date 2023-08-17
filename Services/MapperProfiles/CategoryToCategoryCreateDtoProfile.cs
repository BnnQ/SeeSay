using AutoMapper;
using SeeSay.Models.Dto.Categories;
using SeeSay.Models.Entities;

namespace SeeSay.Services.MapperProfiles;

public class CategoryToCategoryCreateDtoProfile : Profile
{
    public CategoryToCategoryCreateDtoProfile()
    {
        CreateMap<Category, CategoryCreateDto>()
            .ReverseMap();
    }
}