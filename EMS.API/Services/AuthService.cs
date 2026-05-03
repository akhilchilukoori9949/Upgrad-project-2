using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EMS.API.Services;

/// <summary>
/// Handles user registration, login, BCrypt verification, and JWT generation.
/// </summary>
public class AuthService(AppDbContext context, IConfiguration config)
{
    /// <summary>
    /// Registers a new user with a hashed password.
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = request.Username.Trim();
        var normalizedRole = NormalizeRole(request.Role);

        var exists = await context.AppUsers.AnyAsync(
            user => user.Username.ToLower() == normalizedUsername.ToLower(),
            cancellationToken);

        if (exists)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Username already exists. Please choose a different one.",
            };
        }

        var user = new AppUser
        {
            Username = normalizedUsername,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            Role = normalizedRole,
        };

        context.AppUsers.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Registration completed successfully.",
            Username = user.Username,
            Role = user.Role,
            Token = GenerateToken(user),
        };
    }

    /// <summary>
    /// Validates submitted credentials against the stored BCrypt hash.
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();

        var user = await context.AppUsers.FirstOrDefaultAsync(
            item => item.Username.ToLower() == username.ToLower(),
            cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid credentials. Please check your username and password.",
            };
        }

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful.",
            Username = user.Username,
            Role = user.Role,
            Token = GenerateToken(user),
        };
    }

    /// <summary>
    /// Generates a signed JWT that carries the user's id, username, and role.
    /// </summary>
    public string GenerateToken(AppUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryHours = double.Parse(config["Jwt:ExpiryHours"] ?? "8");

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string NormalizeRole(string? role)
    {
        if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return "Admin";
        }

        return "Viewer";
    }
}
