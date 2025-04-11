using AutoMapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRegisterDto, User>();
        CreateMap<User, UserResponseDto>();
    }
}
