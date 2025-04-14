using AutoMapper;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
       CreateMap<Transaction, TransactionResponseDto>()
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account!.Name))
            .ForMember(dest => dest.SubcategoryName, opt => opt.MapFrom(src => src.Subcategory!.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Subcategory!.Category!.Name));


        CreateMap<TransactionCreateDto, Transaction>();

        
    }
}
