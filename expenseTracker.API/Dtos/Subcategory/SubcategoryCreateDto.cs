using System.ComponentModel.DataAnnotations;

public class SubcategoryCreateDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }
}
