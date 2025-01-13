using System.ComponentModel.DataAnnotations;

namespace TaskFleet.DTOs;

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [MinLength(3)]
    public string FirstName { get; set; }

    [Required]
    [MinLength(3)]
    public string LastName { get; set; }

    [Required]
    public string Role { get; set; } 

    public List<int>? LicenseIds { get; set; } = new List<int>(); 
}