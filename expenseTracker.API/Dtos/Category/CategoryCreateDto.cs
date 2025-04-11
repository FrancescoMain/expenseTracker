using System.ComponentModel.DataAnnotations;

public class CategoryCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
