
using AutoMapper;

public class BudgetProfile : Profile
{
    public BudgetProfile()
    {
        CreateMap<BudgetCreateDto, Budget>();

        CreateMap<Budget, BudgetResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name))
            .ForMember(dest => dest.SubcategoryName, opt => opt.MapFrom(src => src.Subcategory!.Name))
            .ForMember(dest => dest.Spent, opt => opt.Ignore()); // sarà calcolato nel servizio
    }
}
