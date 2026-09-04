# Seed Users

## Goal

Replace current seed data (alice, bob) in `InMemoryUserRepository` with `{1, "user1"}` and `{2, "user2"}`.

## Decisions

- Replace, not add alongside: alice/bob removed entirely.
- IDs must be exactly 1 and 2, matching current auto-increment `Add()` behavior (first two calls yield 1, 2) — no repository interface change needed.
- Hub-side "connected users" lookup was discussed but deferred to a future plan (no built-in SignalR API for listing connections; would require a custom tracker). Out of scope here.

## Folder Structure

No structural change. Single file edit:

```
src/StickySessionApi.Infrastructure/Users/InMemoryUserRepository.cs
```

## Implementation Steps

1. In `InMemoryUserRepository` constructor, replace:
   ```csharp
   Add("alice");
   Add("bob");
   ```
   with:
   ```csharp
   Add("user1");
   Add("user2");
   ```
2. Verify `Add()`'s `Interlocked.Increment(ref _nextId)` still yields 1 then 2 (unchanged logic — just confirms no drift).
3. Build (`dotnet build`) to confirm no other code references "alice"/"bob" (e.g. `StickySessionApi.http` sample requests) that need updating for consistency.

## Out of Scope

- Hub connected-users tracking/lookup (deferred — needs custom tracker class, e.g. `ConnectedUsersTracker`, since `IHubContext<MessagingHub>` has no built-in API to enumerate active connections).
- Any REST endpoint exposing connected users.
- Persisting seed data beyond in-memory process lifetime.
