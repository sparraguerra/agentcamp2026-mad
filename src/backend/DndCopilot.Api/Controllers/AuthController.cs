using Microsoft.AspNetCore.Mvc;
using DndCopilot.Api.Models;
using DndCopilot.Api.Services;
using DndCopilot.Core.Entities;
using DndCopilot.Core.Interfaces;

namespace DndCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly AuthService _authService;

    public AuthController(IUserRepository userRepository, AuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Create new user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _authService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        // Generate token
        var token = _authService.GenerateJwtToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = _authService.GenerateJwtToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username
        });
    }
}
