using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskFleet.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
    [DefaultValue("example@domain.com")]
    public string Email { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "First name must be at least {2}, and maximum {1} characters")]
    public string FirstName { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "Last name must be at least {2}, and maximum {1} characters")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} characters")]
    public string Password { get; set; }
}