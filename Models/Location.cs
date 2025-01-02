using System.ComponentModel.DataAnnotations;

namespace TaskFleet.Models;

public class Location
{
    [Key]
    public int LocationId { get; set; }

    [Required] [MaxLength(50)] 
    public string City { get; set; } = string.Empty;
    
}