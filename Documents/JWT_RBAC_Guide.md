# JWT Authentication & Role-Based Authorization Guide

## Overview

TaskFlow implements JWT (JSON Web Token) authentication with role-based access control (RBAC) to secure API endpoints and manage user permissions according to three distinct roles: Admin, Manager, and Developer.

---

## Architecture

### JWT Configuration

The API uses symmetric key encryption with HS256 (HMAC SHA-256) algorithm for JWT token signing. Configuration is defined in `appsettings.json`:

```json
"Jwt": {
  "Key": "YourSuperSecretKeyHere12345678901234567890",
  "Issuer": "TaskFlow",
  "Audience": "TaskFlowUsers"
}
```

**⚠️ Security Note**: Ensure the JWT key is at least 32 characters long and is kept secret in production using environment variables or secure vaults like Azure Key Vault.

### Token Structure

JWT tokens include the following claims:
- `sub` (NameIdentifier): User's unique ID (GUID)
- `name`: Username
- `email`: User's email address
- `role`: User's role (Admin, Manager, or Developer)

**Token Expiration**: 7 days from issuance

Example decoded token payload:
```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "name": "john_doe",
  "email": "john@taskflow.com",
  "role": "Manager",
  "iat": 1704067200,
  "exp": 1704672000,
  "iss": "TaskFlow",
  "aud": "TaskFlowUsers"
}
```

---

## Role-Based Access Control

### Roles & Permissions

| Role | Permissions |
|------|-------------|
| **Admin** | • View/manage all users<br/>• Delete users & assign roles<br/>• Delete projects<br/>• Delete sprints<br/>• Delete tasks<br/>• Full system access |
| **Manager** | • Create & edit sprints<br/>• Create & update projects<br/>• Create & assign tasks<br/>• Update any task<br/>• View all board data |
| **Developer** | • View board & tasks<br/>• Update own assigned tasks<br/>• Add comments to tasks<br/>• View projects & sprints |

### Authorization Policies

Three authorization policies are configured in `Program.cs`:

1. **AdminOnly** - Requires Admin role
2. **ManagerOrAdmin** - Requires Manager or Admin role
3. **DeveloperAndAbove** - Requires Developer, Manager, or Admin role

---

## API Endpoints & Authorization

### Authentication Endpoints

#### POST `/api/auth/login`
**Access**: Public (AllowAnonymous)

Authenticates a user and returns a JWT token.

**Request**:
```json
{
  "username": "john_doe",
  "password": "SecurePassword123"
}
```

**Response** (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john_doe",
  "role": "Manager",
  "userId": "550e8400-e29b-41d4-a716-446655440000"
}
```

#### POST `/api/auth/register`
**Access**: Public (AllowAnonymous)

Creates a new user account (default role: Developer).

**Request**:
```json
{
  "username": "jane_doe",
  "email": "jane@example.com",
  "password": "SecurePassword123"
}
```

**Response** (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "jane_doe",
  "role": "Developer",
  "userId": "660e8400-e29b-41d4-a716-446655440001"
}
```

---

### Projects Endpoints

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/projects` | Authenticated | List all projects |
| GET | `/api/projects/{id}` | Authenticated | Get project by ID |
| POST | `/api/projects` | ManagerOrAdmin | Create new project |
| PUT | `/api/projects/{id}` | ManagerOrAdmin | Update project |
| DELETE | `/api/projects/{id}` | AdminOnly | Delete project |

**Example**: Create project as Manager
```bash
curl -X POST https://api.taskflow.com/api/projects \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Q1 2024 Initiative",
    "description": "Quarterly goals and deliverables"
  }'
```

---

### Sprints Endpoints

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/sprints` | Authenticated | List all sprints |
| GET | `/api/sprints/{id}` | Authenticated | Get sprint by ID |
| POST | `/api/sprints` | ManagerOrAdmin | Create new sprint |
| PUT | `/api/sprints/{id}` | ManagerOrAdmin | Update sprint |
| DELETE | `/api/sprints/{id}` | AdminOnly | Delete sprint |

---

### Tasks Endpoints

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/tasks` | DeveloperAndAbove | List all tasks |
| GET | `/api/tasks/{id}` | DeveloperAndAbove | Get task by ID |
| POST | `/api/tasks` | ManagerOrAdmin | Create new task |
| PUT | `/api/tasks/{id}` | DeveloperAndAbove* | Update task |
| DELETE | `/api/tasks/{id}` | ManagerOrAdmin | Delete task |

**Note**: * Developers can only update tasks assigned to them. Managers & Admins can update any task.

Update validation in code:
```csharp
var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

// Developers can only update their own tasks
if (userRole == "Developer" && task.AssignedUserId.ToString() != userId)
    return Forbid("You can only update your own tasks");
```

---

### Users (Admin) Endpoints

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/users` | AdminOnly | List all users |
| GET | `/api/users/{id}` | AdminOnly | Get user by ID |
| PUT | `/api/users/{id}` | AdminOnly | Update user role/info |
| DELETE | `/api/users/{id}` | AdminOnly | Delete user |

**Update user role** (Admin only):
```bash
curl -X PUT https://api.taskflow.com/api/users/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@taskflow.com",
    "role": "Manager"
  }'
```

---

## Swagger UI Integration

Swagger/OpenAPI documentation is available at:
- **Dev**: `https://localhost:7XXX/` (or `http://localhost:5XXX/swagger/index.html`)
- **Prod**: `https://api.taskflow.com/swagger/index.html`

### Authentication in Swagger

1. Click the green **Authorize** button at the top of the Swagger page
2. In the "Bearer {token}" field, enter: `YOUR_JWT_TOKEN` (without "Bearer" prefix)
3. Click **Authorize** - now all endpoints will include the token

### API Documentation

Each endpoint includes:
- Description of functionality
- Request/response schemas
- Authorization requirements
- Response status codes (200, 401, 403, 404, etc.)

---

## Implementation Details

### AuthService

Located in `TaskFlow.API/Services/AuthService.cs`, handles:
- User authentication with bcrypt password hashing
- JWT token generation with role claims
- User registration with default Developer role

```csharp
public class AuthService : IAuthService
{
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### JWT Middleware

Configured in `Program.cs`:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
```

### Authorization Policies

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"))
    .AddPolicy("DeveloperAndAbove", policy => 
        policy.RequireRole("Developer", "Manager", "Admin"));
```

---

## Client Integration (Angular)

The frontend authenticates by:

1. **Login**: Send credentials to `/api/auth/login`, receive JWT token
2. **Store**: Save token in localStorage
3. **Send**: Include token in `Authorization: Bearer {token}` header for all API requests
4. **Refresh**: Implement token refresh logic when token expires

### Auth Service Example (Angular)

```typescript
export class AuthService {
  loginWithToken(token: string): void {
    localStorage.setItem('token', token);
  }

  getAuthHeader(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }
}
```

---

## Security Considerations

### Development
- Use the provided JWT key (minimum 32 characters)
- Enable HTTPS in production only

### Production Checklist
- ✅ Change JWT key to a strong, randomly generated value (50+ chars)
- ✅ Store JWT key in environment variables or Key Vault
- ✅ Enable HTTPS/TLS for all endpoints
- ✅ Rotate JWT keys periodically
- ✅ Implement token refresh mechanism
- ✅ Set appropriate CORS policies for frontend domain
- ✅ Use secure password hashing (bcrypt is configured)
- ✅ Implement rate limiting for login attempts
- ✅ Log authentication failures for monitoring

### Token Expiration
- **JWT expires after**: 7 days
- **Consider**: Implement shorter expiration (1-2 hours) with refresh tokens for high-security applications

---

## Error Responses

### 401 Unauthorized
No valid JWT token provided or token has expired.
```json
{
  "statusCode": 401,
  "message": "Unauthorized access",
  "details": "The token is invalid or expired"
}
```

### 403 Forbidden
User authenticated but lacks required role/permissions.
```json
{
  "statusCode": 403,
  "message": "Access Denied",
  "details": "Your role does not have permission to access this resource"
}
```

---

## Testing

### Integration Tests
Located in `TaskFlow.Tests/Integration/TasksControllerTests.cs`

Run tests:
```bash
dotnet test
```

---

## References

- [Microsoft ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [JWT.io - JWT Documentation](https://jwt.io)
- [bcrypt - Secure Password Hashing](https://github.com/dcjw/BCrypt.Net-Next)
- [Swagger/OpenAPI](https://swagger.io/)

---

*Last Updated: April 7, 2026*
