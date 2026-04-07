using TaskFlow.API.DTOs;

namespace TaskFlow.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(string username, string password);
    Task<AuthResponseDto?> RegisterAsync(string username, string email, string password);
}