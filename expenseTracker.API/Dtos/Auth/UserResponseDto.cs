public class UserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string Token { get; set; } = string.Empty;
}
