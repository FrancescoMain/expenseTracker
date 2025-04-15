using AutoMapper;

public class TransferProfile : Profile
{
    public TransferProfile()
    {
        CreateMap<TransferCreateDto, Transfer>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date ?? DateTime.UtcNow));

        CreateMap<Transfer, TransferResponseDto>()
            .ForMember(dest => dest.FromAccountName, opt => opt.MapFrom(src => src.FromAccount!.Name))
            .ForMember(dest => dest.ToAccountName, opt => opt.MapFrom(src => src.ToAccount != null ? src.ToAccount.Name : null))
            .ForMember(dest => dest.SavingGoalName, opt => opt.MapFrom(src => src.SavingGoal != null ? src.SavingGoal.Name : null));

    }
}
