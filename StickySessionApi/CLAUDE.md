# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Core Web API on .NET 10, Clean Architecture layout across 4 projects. Has a `User` entity, SignalR hub for server-to-client messaging, and an instance-GUID banner/log for tracking which app instance a connection landed on (groundwork for sticky sessions — no actual routing/proxy logic yet). Nullable and ImplicitUsings enabled throughout.

## Architecture

```
src/
  StickySessionApi.Domain/          # Entities.User (Id:int, Username:string). No dependencies.
  StickySessionApi.Application/     # Interfaces: Users.IUserRepository, Messaging.IMessageSender, Messaging.SendMessageRequest. Depends on Domain.
  StickySessionApi.Infrastructure/  # Users.InMemoryUserRepository (ConcurrentDictionary, seeded w/ alice+bob).
                                     # Messaging.MessagingHub (SignalR Hub — logs connect/disconnect w/ instance id),
                                     # Messaging.SignalRUserIdProvider (maps ?userId= query param -> Hub user id),
                                     # Messaging.SignalRMessageSender (IMessageSender impl over IHubContext<MessagingHub>),
                                     # Messaging.InstanceId (singleton, one Guid per process).
                                     # Messaging.SignalRServiceExtensions.AddMessagingSignalR() -> wires up SignalR, optionally
                                     #   w/ StackExchangeRedis backplane (Messaging.RedisOptions, config section "Redis": Enabled/ConnectionString).
                                     #   Off by default (appsettings.json Redis.Enabled=false) -> multi-instance without Redis means
                                     #   clients only get messages if connected to the instance handling the send.
                                     # Depends on Application; needs FrameworkReference Microsoft.AspNetCore.App for SignalR types.
  StickySessionApi.Api/             # Program.cs composition root, minimal API endpoints, hub mapping, startup banner.
                                     # Endpoints.UsersEndpoints -> GET/POST /users
                                     # Endpoints.MessagesEndpoints -> POST /messages (goes through IMessageSender -> SignalR hub, client never calls hub directly to send)
                                     # Startup.InstanceBanner -> Figgle ASCII banner + instance GUID, printed once at boot
                                     # Hub mapped at /hubs/messaging
```

Dependency direction: Api -> Infrastructure -> Application -> Domain.

Storage is in-memory only (no DB). No auth — `userId` is passed unauthenticated via SignalR connection query string and in the `/messages` request body.

## Commands

Run from repo root.

- Build: `dotnet build`
- Run: `dotnet run --project src/StickySessionApi.Api` (profile `http` → `http://localhost:5001`; profile `https` adds `https://localhost:7088`)
- Run https profile: `dotnet run --project src/StickySessionApi.Api --launch-profile https`
- Restore: `dotnet restore`

No test project exists yet. OpenAPI doc served at `/openapi/v1.json` (dev only, `MapOpenApi`).

## Notes

- `src/StickySessionApi.Api/StickySessionApi.http` has sample requests for `/users` and `/messages`.
- Minimal API style (endpoints in `Endpoints/*.cs`, mapped from `Program.cs`), not controller-based.
- To send a message to a connected user: connect a SignalR client to `/hubs/messaging?userId=<id>`, then `POST /messages` with `{ "userId": <id>, "message": "..." }`. Client listens for the `"ReceiveMessage"` hub method.
- Sticky-session routing/proxy affinity is NOT implemented — current scope is log-only (`MessagingHub.OnConnectedAsync`/`OnDisconnectedAsync` log the instance GUID + user id per connection).
- Redis SignalR backplane (`Messaging.RedisOptions`) exists but is disabled by default; enable via `Redis:Enabled=true` + `Redis:ConnectionString` to fan out messages across instances.
- Feature plans live in `.claude/plans/<feature-name>/plan.md`; use the `feature-planning` skill (`.claude/skills/feature-planning/`) to create new ones.
