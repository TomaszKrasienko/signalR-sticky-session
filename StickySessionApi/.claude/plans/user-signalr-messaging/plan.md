# User SignalR Messaging + Instance Banner

## Goal
Add `User` entity (Id:int, Username:string), SignalR hub for sending messages to a specific user via REST trigger, and a startup banner/logger showing app instance GUID for tracking sticky-session connections. Restructure into Clean Architecture folders.

## Decisions
- **Banner lib**: Figgle (ASCII-art banner with instance GUID printed to console at startup).
- **User mapping (SignalR)**: Custom `IUserIdProvider` returning `User.Id.ToString()` — enables `Clients.User(id)`.
- **Storage**: In-memory (no DB yet) — simple repository/collection abstraction so EF Core can be swapped in later.
- **Sticky-session scope (this iteration)**: Log-only. On hub connect/disconnect, log connection with instance GUID. No actual routing/proxy logic yet.
- **Message send trigger**: `POST /messages` REST endpoint. Handler resolves `IHubContext<MessagingHub>` and calls `Clients.User(userId).SendAsync(...)` — client does not call the hub directly to send.
- **Tests**: Skipped for this iteration.
- **CLAUDE.md**: Update with full architecture/folder map + run commands after implementation.

## Folder Structure
```
StickySessionApi.sln
src/
  StickySessionApi.Domain/
    Entities/
      User.cs
    StickySessionApi.Domain.csproj
  StickySessionApi.Application/
    Users/
      IUserRepository.cs
    Messaging/
      IMessageSender.cs
      SendMessageRequest.cs
    StickySessionApi.Application.csproj
  StickySessionApi.Infrastructure/
    Users/
      InMemoryUserRepository.cs
    Messaging/
      SignalRUserIdProvider.cs
      SignalRMessageSender.cs
    StickySessionApi.Infrastructure.csproj
  StickySessionApi.Api/
    Hubs/
      MessagingHub.cs
    Endpoints/
      MessagesEndpoints.cs
      UsersEndpoints.cs
    Startup/
      InstanceBanner.cs
    Program.cs
    StickySessionApi.Api.csproj
```
(`StickySessionApi.Api` replaces current root `StickySessionApi` project — move existing `Program.cs` content in.)

## Implementation Steps
1. Create solution file `StickySessionApi.sln`, add 4 projects (Domain/Application/Infrastructure/Api), wire project references: Api → Infrastructure → Application → Domain.
2. **Domain**: add `User` record/class (Id:int, Username:string).
3. **Application**: define `IUserRepository` (GetById, GetAll, Add), `IMessageSender.SendToUserAsync(int userId, string message)`, `SendMessageRequest` DTO.
4. **Infrastructure**:
   - `InMemoryUserRepository` — `ConcurrentDictionary<int, User>`, seed a couple sample users.
   - `SignalRUserIdProvider : IUserIdProvider` — maps `HttpContext` query/claim (define a simple `userId` query param or header for connection, since no auth yet) to `User.Id`.
   - `SignalRMessageSender : IMessageSender` — wraps `IHubContext<MessagingHub>`, calls `Clients.User(id).SendAsync("ReceiveMessage", message)`.
5. **Api**:
   - `MessagingHub : Hub` — `OnConnectedAsync`/`OnDisconnectedAsync` log connection id + resolved user id + instance GUID.
   - `POST /messages` minimal API endpoint — body `{ userId, message }`, calls `IMessageSender.SendToUserAsync`.
   - `GET /users`, `POST /users` minimal endpoints for basic User CRUD (create/list) to support testing message sending.
   - `InstanceBanner` — static class, `Guid.NewGuid()` generated once at startup, printed via Figgle (`FiggleFonts.Standard.Render(...)`) + plain log line with the GUID, called first thing in `Program.cs`.
   - Update `Program.cs`: register DI (`IUserRepository`, `IMessageSender`, `IUserIdProvider`), `AddSignalR()`, map hub at `/hubs/messaging`, map new endpoints, call `InstanceBanner.Print()`.
6. Add Figgle NuGet package to `StickySessionApi.Api`.
7. Update `StickySessionApi.http` with sample requests for `POST /users` and `POST /messages` (remove stale `/weatherforecast/` reference).
8. Build (`dotnet build`) and smoke-test manually: run, create user, connect a SignalR client (or check hub via browser console) — confirm log shows instance GUID + connection.

## Out of Scope
- Real sticky-session routing / reverse proxy / load balancer affinity logic.
- Persistent storage (EF Core/DB).
- Auth/identity (JWT etc.) — user id passed unauthenticated for now.
- Unit/integration tests.
- Multi-instance deployment/testing.
