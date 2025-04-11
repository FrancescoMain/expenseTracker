using expenseTracker.API.Models;

public interface IAuthService
{
    Task<ServiceResponse<UserResponseDto>> Register(UserRegisterDto dto);
    Task<ServiceResponse<UserResponseDto>> Login(UserLoginDto dto);
}
