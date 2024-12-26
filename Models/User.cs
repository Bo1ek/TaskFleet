using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TaskFleet.Models;

public class User : IdentityUser
{
    [PersonalData]
    [Required]
    [MinLength(3)]
    [MaxLength(255)]
    public string? FirstName { get; set; }
    [PersonalData]
    [Required]
    [MinLength(3)]
    [MaxLength(255)]
    public string? LastName { get; set; }
}