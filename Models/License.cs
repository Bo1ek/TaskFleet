using System.ComponentModel.DataAnnotations;

namespace TaskFleet.Models;

public class License
{
    [Key]
    public int LicenseId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Category { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}