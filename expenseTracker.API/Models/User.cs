using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "user"; // "admin" o "user"

    // Navigazione
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
