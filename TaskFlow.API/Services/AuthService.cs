using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.API.DTOs;
using TaskFlow.API.Interfaces;
using TaskFlow.Core.Entities;
using TaskFlow.Core.Interfaces;

namespace TaskFlow.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        var token = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            UserId = user.Id
        };
    }

    public async Task<AuthResponseDto?> RegisterAsync(string username, string email, string password)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
            return null;

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password)
        };

        await _userRepository.AddAsync(user);
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            UserId = user.Id
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "defaultkey"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        // Simple hash for demo - use proper hashing in production
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}