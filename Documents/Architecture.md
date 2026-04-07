# TaskFlow — Team Task & Sprint Manager
### Architecture & Project Structure

---

## Overview

**TaskFlow** is a lightweight, real-time team task and sprint management system built for small teams who need affordable project tracking without the complexity of enterprise tools like Jira.

**Tech Stack:**
- Backend: .NET 9 Web API
- Frontend: Angular 21
- Database: SQLite (via EF Core)
- Real-time: SignalR
- Auth: JWT Bearer Tokens

---

## Solution Structure

```
TaskFlow/                          ← solution root
├── TaskFlow.API/                  ← .NET 9 Web API project
├── TaskFlow.Core/                 ← domain entities, interfaces
├── TaskFlow.Infrastructure/       ← EF Core, SQLite, repositories
├── TaskFlow.Tests/                ← xUnit unit & integration tests
├── taskflow-ui/                   ← Angular 21 frontend
└── TaskFlow.sln
```

---

## Backend — Layer Breakdown

### 1. TaskFlow.Core (Domain Layer)

The pure domain layer. No dependencies on any other project — just C# entities, enums, and repository interfaces.

```
TaskFlow.Core/
├── Entities/
│   ├── User.cs
│   ├── Project.cs
│   ├── Sprint.cs
│   ├── TaskItem.cs
│   └── Comment.cs
├── Enums/
│   ├── TaskStatus.cs         ← Todo, InProgress, Done
│   ├── TaskPriority.cs       ← Low, Medium, High, Critical
│   └── UserRole.cs           ← Admin, Manager, Developer
└── Interfaces/
    ├── ITaskRepository.cs
    ├── ISprintRepository.cs
    ├── IProjectRepository.cs
    └── IUserRepository.cs
```

**Why this layer exists:** Keeps your business rules and domain models completely independent. You can swap SQLite for PostgreSQL later without touching this layer at all.

---

### 2. TaskFlow.Infrastructure (Data Layer)

Implements the repository interfaces defined in Core. All EF Core and SQLite concerns live here.

```
TaskFlow.Infrastructure/
├── Data/
│   └── AppDbContext.cs       ← EF Core DbContext configured for SQLite
├── Migrations/               ← EF Core auto-generated migrations
└── Repositories/
    ├── TaskRepository.cs
    ├── SprintRepository.cs
    ├── ProjectRepository.cs
    └── UserRepository.cs
```

**Key point:** Only this layer knows about SQLite. The API layer talks to `ITaskRepository`, not `TaskRepository` directly.

---

### 3. TaskFlow.API (Presentation Layer)

The outermost layer — exposes REST endpoints, handles JWT auth, and broadcasts real-time events via SignalR.

```
TaskFlow.API/
├── Controllers/
│   ├── AuthController.cs     ← login, register, token refresh
│   ├── ProjectsController.cs
│   ├── SprintsController.cs
│   ├── TasksController.cs
│   └── UsersController.cs
├── DTOs/
│   ├── TaskDto.cs
│   ├── SprintDto.cs
│   ├── ProjectDto.cs
│   └── AuthDto.cs
├── Hubs/
│   └── BoardHub.cs           ← SignalR hub for real-time board sync
├── Services/
│   ├── TaskService.cs
│   ├── SprintService.cs
│   ├── AuthService.cs
│   └── TokenService.cs       ← JWT generation & validation
├── Middleware/
│   └── ExceptionMiddleware.cs
├── Program.cs
└── appsettings.json
```

**Star feature — BoardHub.cs:** When one user drags a card to a new column, every other user on the same board sees it move in real time via SignalR. No polling, no page refresh.

---

### 4. TaskFlow.Tests

```
TaskFlow.Tests/
├── Unit/
│   ├── TaskServiceTests.cs
│   └── SprintServiceTests.cs
└── Integration/
    └── TasksControllerTests.cs
```

---

## Frontend — Angular 21 Structure

```
taskflow-ui/
└── src/app/
    ├── core/
    │   ├── guards/
    │   │   ├── auth.guard.ts           ← blocks unauthenticated access
    │   │   └── role.guard.ts           ← blocks unauthorized roles
    │   ├── interceptors/
    │   │   ├── auth.interceptor.ts     ← attaches JWT to every request
    │   │   └── error.interceptor.ts    ← global HTTP error handling
    │   └── services/
    │       ├── auth.service.ts
    │       └── signalr.service.ts      ← manages SignalR connection lifecycle
    ├── features/
    │   ├── auth/                       ← login, register components
    │   ├── dashboard/                  ← summary cards, sprint velocity chart
    │   ├── board/                      ← kanban columns, CDK drag-drop
    │   ├── sprints/                    ← sprint list, plan sprint, backlog
    │   ├── tasks/                      ← task detail, create, edit, comments
    │   └── projects/                   ← project list, members, settings
    ├── shared/
    │   ├── components/
    │   │   ├── avatar/
    │   │   ├── badge/
    │   │   ├── modal/
    │   │   └── priority-chip/
    │   └── models/
    │       ├── task.model.ts
    │       └── sprint.model.ts
    └── app.routes.ts                   ← lazy-loaded feature routes
```

**Folder philosophy:**
- `core/` — singleton services and guards, imported once in `AppModule`
- `features/` — each feature is a self-contained lazy-loaded module
- `shared/` — dumb, reusable UI components with no business logic

---

## Key Packages

### Backend (NuGet)

| Package | Purpose |
|---|---|
| `Microsoft.EntityFrameworkCore.Sqlite` | SQLite database via EF Core |
| `Microsoft.AspNetCore.SignalR` | Real-time board updates |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT auth middleware |
| `AutoMapper` | Entity ↔ DTO mapping |
| `FluentValidation.AspNetCore` | Request validation |
| `xUnit` + `Moq` | Unit & integration testing |

### Frontend (npm)

| Package | Purpose |
|---|---|
| `@angular/cdk` | Drag-and-drop for Kanban board |
| `@microsoft/signalr` | SignalR client for real-time sync |
| `@angular/material` | UI component library |

---

## Architecture Data Flow

```
Angular 21 (Browser)
       │
       │  HTTP (REST) + WebSocket (SignalR)
       ▼
TaskFlow.API
   ├── Controllers   →  validate request (FluentValidation)
   ├── Services      →  business logic
   ├── BoardHub      →  broadcast real-time events to all board members
   └── DTOs / AutoMapper
       │
       │  Repository pattern (interfaces from Core)
       ▼
TaskFlow.Infrastructure
   ├── AppDbContext  →  EF Core
   └── Repositories  →  CRUD queries
       │
       ▼
   SQLite (.db file on disk)
```

---

## Real-Time Flow (SignalR)

```
User A drags Task card → column "In Progress"
       │
       ▼
PATCH /api/tasks/{id}  (HTTP)
       │
       ▼
TaskService updates DB
       │
       ▼
BoardHub.BroadcastTaskMoved(taskId, newStatus)
       │
  ┌────┴────┐
  ▼         ▼
User B    User C
(card moves live, no refresh)
```

---

## Role-Based Access

| Role | Permissions |
|---|---|
| Admin | Full access — manage members, delete projects |
| Manager | Create/edit sprints, assign tasks, view all |
| Developer | View board, update own tasks, add comments |

---

## Cost

Everything in this stack is **100% free and open source.**

| Tool | License | Cost |
|---|---|---|
| .NET 9 SDK | MIT | Free |
| EF Core + SQLite | MIT | Free |
| SignalR | MIT | Free |
| Angular 21 | MIT | Free |
| Angular CDK | MIT | Free |
| JWT, AutoMapper, FluentValidation | MIT/Apache | Free |
| xUnit, Moq | MIT | Free |
| VS Code | MIT | Free |

For deployment (optional), free tiers are available on **Railway / Render** (API) and **Vercel / Netlify** (Angular frontend).

---

*Generated for TaskFlow — a resume-ready .NET 9 + Angular 21 + SQLite project.*