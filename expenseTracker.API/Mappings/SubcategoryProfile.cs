using AutoMapper;

public class SubcategoryProfile : Profile
{
    public SubcategoryProfile()
    {
        CreateMap<Subcategory, SubcategoryResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name));

        CreateMap<SubcategoryCreateDto, Subcategory>();
    }
}
