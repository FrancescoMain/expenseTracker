using System.ComponentModel.DataAnnotations;

public class AccountCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "bank"; // banca, contanti, prepagata...

    [Range(0, double.MaxValue)]
    public decimal InitialBalance { get; set; }
}
