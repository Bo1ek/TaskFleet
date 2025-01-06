using System.ComponentModel.DataAnnotations;

namespace TaskFleet.Models;

public class Location
{
    [Key]
    public int LocationId { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}
