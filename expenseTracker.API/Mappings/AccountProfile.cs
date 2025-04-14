using AutoMapper;

public class AccounProfile : Profile
{
    public AccounProfile()
    {
        CreateMap<Account, AccountResponseDto>()
            .ForMember(dest => dest.CurrentBalance,
                    opt => opt.MapFrom(src =>
                        src.InitialBalance + src.Transactions.Sum(t => t.Amount)));
        CreateMap<AccountCreateDto, Account>();
    }
}
