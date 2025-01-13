using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Data;
using TaskFleet.DTOs;
using TaskFleet.Interfaces;
using TaskFleet.Models;

namespace TaskFleet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    public AccountsController(SignInManager<User> signInManager, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ITokenService tokenService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(model.Role))
                return BadRequest(new {Message = "Role is required"});
            
            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
            if (!roleExists)
                return BadRequest(new {Message = $"Role {model.Role} does not exist"});
            
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            
            var createdUser = await _userManager.CreateAsync(user, model.Password);

            if (createdUser.Succeeded)
            {
                var role = string.IsNullOrEmpty(model.Role) ? "User" : model.Role;
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (roleResult.Succeeded)
                {
                    return Ok(
                        new NewUserDto
                        {
                            Email = user.Email,
                            Token = _tokenService.CreateToken(user),
                        });
                }
                else
                {
                    return StatusCode(500, roleResult.Errors);
                }
            }
            else
            {
                return StatusCode(500,createdUser.Errors);
            }
            
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == model.Email.ToLower());
        
        if (user == null) 
            return Unauthorized("Invalid Email!");
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Username not found and/or password is incorrect!");

        return Ok(
            new NewUserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
            }
        );
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { Message = "Logged out successfully" });
    }
    
    [HttpGet("protected")]
    [Authorize]
    public IActionResult GetProtected()
    {
        return Ok(new { Message = "Access granted to protected endpoint." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
    {
        try
        {
            var dbUser = await _context.Users.FindAsync(id);
            if (dbUser == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            dbUser.FirstName = updatedUser.FirstName;
            dbUser.LastName = updatedUser.LastName;
            dbUser.Email = updatedUser.Email;

            await _context.SaveChangesAsync();
            return Ok(dbUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }

    
    [HttpGet("details")]
    public async Task<IActionResult> GetUserDetails([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(new { Message = "Email is required" });
        }

        try
        {
            var user = await _context.Users
                .Where(u => u.Email.ToLower() == email.ToLower())
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(email));

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                Role = roles.FirstOrDefault()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            string idAsString = id.ToString();

            var user = await _context.Users.FindAsync(idAsString);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserById: {ex.Message}");
            return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
        }
    }
    
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        var roles = new List<string> { "SuperAdmin", "Admin", "Client", "Hr", "Driver" }; 
        return Ok(roles);
    }
    
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        if (!string.IsNullOrEmpty(model.Role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);
        }

        if (model.LicenseIds != null && model.LicenseIds.Any())
        {
            foreach (var licenseId in model.LicenseIds)
            {
                var license = await _context.Licenses.FindAsync(licenseId);
                if (license == null)
                    return NotFound($"License ID {licenseId} not found.");

                user.Licenses ??= new List<License>();
                user.Licenses.Add(license);
            }

            await _context.SaveChangesAsync();
        }

        return Ok(new { Message = "User created successfully!" });
    }




}