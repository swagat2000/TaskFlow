using TaskFlow.Core.Enums;

namespace TaskFlow.API.DTOs;

public class UserUpdateDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
}