using AutoMapper;


public class SavingGoalProfile : Profile
{
    public SavingGoalProfile()
    {
        CreateMap<SavingGoalCreateDto, SavingGoal>();
        CreateMap<SavingGoal, SavingGoalResponseDto>()
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account!.Name))
            .ForMember(dest => dest.CurrentAmount, opt => opt.MapFrom(src => src.Transfers.Sum(t => t.Amount)));
    }
}
