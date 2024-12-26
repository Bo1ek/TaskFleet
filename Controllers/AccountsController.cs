﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFleet.DTOs;
using TaskFleet.Models;

namespace TaskFleet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountsController(SignInManager<User> signInManager, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var userToAdd = new User
        {
            FirstName = model.FirstName.ToLower(),
            LastName = model.LastName.ToLower(),
            UserName = model.Email.ToLower(),
            Email = model.Email.ToLower(),
        };
        // check if the user exists
        var result = await _userManager.CreateAsync(userToAdd, model.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        // Check if the role exists
        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.Role));
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
        }

        // Assign the user to the role
        var addToRoleResult = await _userManager.AddToRoleAsync(userToAdd, model.Role);
        if (!addToRoleResult.Succeeded) return BadRequest("Failed to add user to role");

        return Ok(new
        {
            Message = "User registered and assigned to the role successfully",
            UserId = userToAdd.Id,
            Role = model.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) 
            return Unauthorized(new { Message = "Invalid email or password" });
        // Check the password
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(new { Message = "Invalid email or password" });
        return Ok(new
        {
            Message = "Login successful",
            UserId = user.Id,
            Email = user.Email
        });
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
}