using AutoMapper;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using ExpencseTracker.Helpers;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;



    public AuthService(AppDbContext context, IMapper mapper, JwtSettings jwtSettings)
    {
        _context = context;
        _mapper = mapper;
        _jwtSettings = jwtSettings;

    }

    

    public async Task<ServiceResponse<UserResponseDto>> Register(UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
        {
            return new ServiceResponse<UserResponseDto>
            {
                Success = false,
                Message = "Email già in uso",
                StatusCode = 400
            };
        }

        var user = _mapper.Map<User>(dto);
        user.Email = user.Email.ToLower();
        user.PasswordHash = HashPassword(dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

       var tokenString = GenerateJwtToken(user);

        var responseDto = _mapper.Map<UserResponseDto>(user);
        responseDto.Token = tokenString;

        return new ServiceResponse<UserResponseDto> { Data = responseDto };
    }

    public async Task<ServiceResponse<UserResponseDto>> Login(UserLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
        {
            
            return new ServiceResponse<UserResponseDto>
            {
                Success = false,
                Message = "Credenziali non valide",
                StatusCode = 401
            };
        }

        var tokenString = GenerateJwtToken(user);
        var responseDto = _mapper.Map<UserResponseDto>(user);
        responseDto.Token = tokenString;
        return new ServiceResponse<UserResponseDto> { Data = responseDto };
    }

    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        return HashPassword(password) == storedHash;
    }

    private string GenerateJwtToken(User user)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
}

