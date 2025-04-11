using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
