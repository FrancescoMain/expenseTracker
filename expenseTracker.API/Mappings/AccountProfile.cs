using AutoMapper;

public class AccounProfile : Profile
{
    public AccounProfile()
    {
        CreateMap<Account, AccountResponseDto>();
        CreateMap<AccountCreateDto, Account>();
    }
}
