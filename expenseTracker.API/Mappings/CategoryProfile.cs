using AutoMapper;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<Category, CategoryResponseDto>();
    }
}
