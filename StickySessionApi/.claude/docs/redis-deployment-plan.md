# Redis deployment + feature-switch plan

## Goal

Add Redis to k8s deploy. Config (SignalR: no-Redis vs Redis backplane) live in ConfigMap — easy toggle + rollout, no image rebuild.

## Current state

- `Redis:Enabled` / `Redis:ConnectionString` already read from config (`RedisOptions.cs`, `SignalRServiceExtensions.AddMessagingSignalR()`).
- `appsettings.json` hardcodes `Redis.Enabled=false`.
- `deploy/deployment.yaml` — no env vars, no ConfigMap, no Redis pod/service.

Feature switch already exists in code. Missing piece = deploy-time config + Redis infra itself.

## Plan

### 1. ConfigMap for app config
New `deploy/configmap.yaml`:
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: stickysessionapi-config
data:
  Redis__Enabled: "false"
  Redis__ConnectionString: "redis:6379"
```
(`__` = ASP.NET Core env-var config separator, maps to `Redis:Enabled` / `Redis:ConnectionString`.)

### 2. Wire ConfigMap into Deployment
Edit `deploy/deployment.yaml`, add to container spec:
```yaml
          envFrom:
            - configMapRef:
                name: stickysessionapi-config
```
Toggle Redis on/off = edit ConfigMap value, `kubectl rollout restart deployment/stickysessionapi` (env vars don't hot-reload, pod restart needed — that's the "rollout" the user wants).

### 3. Redis deployment + service
New `deploy/redis-deployment.yaml`:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: redis
  labels:
    app: redis
spec:
  replicas: 1
  selector:
    matchLabels:
      app: redis
  template:
    metadata:
      labels:
        app: redis
    spec:
      containers:
        - name: redis
          image: redis:7-alpine
          ports:
            - containerPort: 6379
---
apiVersion: v1
kind: Service
metadata:
  name: redis
spec:
  selector:
    app: redis
  ports:
    - port: 6379
      targetPort: 6379
```
Service name `redis` matches ConfigMap's `Redis__ConnectionString: redis:6379` (in-cluster DNS).

No persistence needed — Redis here is only SignalR backplane (pub/sub), not durable store. Plain `Deployment`, not `StatefulSet`.

### 4. Local dev parity (optional, not blocking)
`appsettings.Development.json` / docker-compose (if added later) can mirror same Redis service for local multi-instance testing.

## Rollout flow (after this lands)

1. Edit `deploy/configmap.yaml` → `Redis__Enabled: "true"`.
2. `kubectl apply -f deploy/configmap.yaml`
3. `kubectl rollout restart deployment/stickysessionapi`

Same to switch back off.

## Files to touch
- `deploy/configmap.yaml` (new)
- `deploy/redis-deployment.yaml` (new)
- `deploy/deployment.yaml` (add `envFrom`)
- update `CLAUDE.md` deploy notes after impl (mention ConfigMap-driven Redis toggle)

## Out of scope
- Sticky-session routing/affinity (still not implemented, per CLAUDE.md).
- Redis auth/TLS/persistence — add later if needed for prod.
