using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.IAM.Api.Auth;
using Orchestra.IAM.Application.Contracts;
using Orchestra.IAM.Domain.Entities;
using Orchestra.IAM.Infrastructure.Data;

namespace Orchestra.IAM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IamDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthController(IamDbContext context, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var existingUser = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser is not null)
        {
            return Conflict("User with this email already exists.");
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        var response = new UserResponse(user.Id, user.Email);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new LoginResponse(token));
    }
}