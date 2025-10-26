# Service Lifecycle & Registration Architecture

## Overview
Architecture centralisée pour gérer l'enregistrement et le cycle de vie des orchestrateurs et agents workers avec notifications temps réel via WebSocket.

## Architecture

```
???????????????????????????????????????????????????????????????
?   Aspire AppHost   ?
?  - Configure et démarre tous les services           ?
?  - Passe RegistryApiUrl à tous les services  ?
???????????????????????????????????????????????????????????????
             ?
    ????????????????????????????????????????????????????????
    ?        ?         ?             ?       ?
??????????   ????????????  ?????????  ??????????????  ????????
?Registry?   ?Orchestr  ?  ? Viz   ?  ?Agent 1-N   ?  ? ...  ?
?  API   ?   ? -ator    ?  ?  UI   ?  ?            ?  ?      ?
?        ?????  ?  ?       ?  ?            ?  ? ?
? (Viz.  ?   ?          ?  ?       ?  ?   ?  ?      ?
?  Api)  ?????????????????????????????????????????????????????
??????????      Register     WebSocketRegister
    ?           Heartbeat    Subscribe     Heartbeat
    ?
    ??? Broadcast events via WebSocket
```

## Resilience & Error Handling

### Robust Registration with Retry Logic

Les services d'enregistrement (Orchestrator et Agent) implémentent une logique de retry robuste:

**Retry Policy**:
- **Max attempts**: 5 tentatives
- **Exponential backoff**: 5s ? 10s ? 20s ? 40s ? 80s (max 120s)
- **Graceful degradation**: Le service continue à fonctionner même si l'enregistrement échoue

**Gestion des erreurs**:
1. **Network Failures** (HttpRequestException)
   - Pas de connexion Internet
   - Erreur DNS
   - API Registry injoignable
 - ? Retry automatique avec backoff

2. **Timeouts** (TaskCanceledException)
   - Timeout HTTP (10s par défaut)
   - API Registry trop lente
   - ? Retry automatique avec backoff

3. **HTTP 404 Not Found**
   - Endpoint inexistant
   - URL RegistryApiUrl incorrecte
   - ? Log erreur, retry (peut être déploiement en cours)

4. **HTTP 503 Service Unavailable**
   - API Registry surchargée ou en redémarrage
   - ? Retry automatique avec backoff

5. **Configuration Missing**
   - `RegistryApiUrl` non configurée
   - `ASPNETCORE_URLS` manquante
   - ? Log erreur, pas de retry (problème de config)

6. **Invalid Response**
   - Response JSON malformée
   - ID manquant dans la réponse
   - ? Log erreur, retry

**Heartbeat Resilience**:
- Surveillance des échecs consécutifs (max 3)
- Après 3 échecs consécutifs ? tentative de re-registration automatique
- Heartbeat failures non critiques (logged as Warning)
- Service continue même sans heartbeat

**Shutdown Graceful**:
- Unregister en best-effort (pas d'échec si API unavailable)
- Timeout court pour ne pas bloquer le shutdown
- Logs clairs pour debugging

### Logs par Niveau

**Information**:
- Registration attempt X/5
- Successfully registered on attempt X
- Orchestrator/Agent registered with ID: {id}
- Status updated to {status}
- Unregistered successfully

**Warning**:
- Failed to register after 5 attempts. Service will continue without registration.
- Network error during registration (retry X/5)
- Heartbeat network error (X/3 consecutive failures)
- Too many consecutive heartbeat failures. Attempting re-registration...
- Network error during unregister (non-critical)

**Error**:
- RegistryApiUrl not configured
- ASPNETCORE_URLS not configured
- Registration endpoint not found (404)
- Failed to register after 5 attempts

**Debug**:
- Skipping heartbeat - not registered
- Heartbeat sent for orchestrator {id}

## Components

### 1. Registry API (`FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api`)

**Port**: 5060 (configurable via `DnsConfiguration.Ports.AppHost`)  
**URL**: `https://apphost.pd3i1.com:5060`

**Services**:
- `OrchestratorRegistry` - Registre singleton pour orchestrateurs
- `AgentRegistry` - Registre singleton pour agents
- `RegistryWebSocketManager` - Gestionnaire WebSocket pour broadcasts temps réel

**Endpoints REST**:

#### Orchestrators
- `GET /api/orchestrators` - Liste tous les orchestrateurs
- `POST /api/orchestrators/register` - Enregistrer un orchestrateur
  ```json
  { "url": "https://orchestrator.pd3i1.com:5080" }
  ```
- `POST /api/orchestrators/{id}/unregister` - Désenregistrer
- `POST /api/orchestrators/{id}/heartbeat` - Envoyer heartbeat
- `POST /api/orchestrators/{id}/status` - Mettre à jour le statut
  ```json
  { "status": "Running" }
  ```

#### Agents
- `GET /api/agents` - Liste tous les agents
- `GET /api/agents/orchestrator/{orchestratorId}` - Agents par orchestrateur
- `POST /api/agents/register` - Enregistrer un agent
  ```json
  { "orchestratorId": "abc123", "url": "https://worker-agent-1.pd3i1.com:5090" }
  ```
- `POST /api/agents/{id}/unregister` - Désenregistrer
- `POST /api/agents/{id}/heartbeat` - Envoyer heartbeat
- `POST /api/agents/{id}/status` - Mettre à jour le statut

#### WebSocket
- `GET /ws` - Endpoint WebSocket pour notifications temps réel

### 2. Orchestrator (`FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Orchestrator`)

**Port**: 5080  
**Variables d'environnement**:
- `RegistryApiUrl` - URL de l'API Registry

**Comportement**:
1. Au démarrage: 5 tentatives d'enregistrement avec backoff exponentiel
2. Si échec après 5 tentatives: continue sans enregistrement (mode dégradé)
3. Si succès: heartbeat toutes les 30s
4. Surveillance des échecs heartbeat (max 3 consécutifs)
5. Re-registration automatique après 3 échecs consécutifs
6. Au shutdown: unregister en best-effort

### 3. Worker Agent (`FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Agent`)

**Ports**: 5090+ (base + index)  
**Variables d'environnement**:
- `RegistryApiUrl` - URL de l'API Registry
- `OrchestratorUrl` - URL de l'orchestrateur (pour coordination)

**Comportement**: Identique à l'Orchestrator avec orchestratorId

### 4. Viz UI (`FrenchExDev.Net.CSharp.ProjectDependency3.Viz`)

**Port**: 5070  
**Page**: `/lifecycle`  
**Variables d'environnement**:
- `RegistryApiUrl` - URL de l'API Registry

**Fonctionnalités**:
- Affichage temps réel des orchestrateurs et agents
- Tables avec ID, URL, Status, Last Heartbeat
- WebSocket pour mises à jour automatiques
- Badges colorés par statut

## Data Models

### OrchestratorRegistration
```csharp
record OrchestratorRegistration(
    string Id,       // UUID
    string Url,// https://orchestrator.pd3i1.com:5080
    DateTime RegisteredAt,        // UTC
    DateTime LastHeartbeat,    // UTC
    OrchestratorStatus Status);   // Starting|Running|Stopping|Stopped|Failed
```

### AgentRegistration
```csharp
record AgentRegistration(
    string Id,       // UUID
    string OrchestratorId,     // UUID de l'orchestrateur
    string Url,                // https://worker-agent-1.pd3i1.com:5090
    DateTime RegisteredAt,     // UTC
    DateTime LastHeartbeat,    // UTC
    AgentStatus Status); // Starting|Idle|Busy|Stopping|Stopped|Failed
```

## WebSocket Message Format

Tous les messages WebSocket suivent ce format:
```json
{
  "type": "event_type",
  "data": { ...registration object... }
}
```

**Event Types**:
- `orchestrator_registered` - Nouvel orchestrateur enregistré
- `orchestrator_unregistered` - Orchestrateur désenregistré
- `orchestrator_status_changed` - Statut orchestrateur changé
- `agent_registered` - Nouvel agent enregistré
- `agent_unregistered` - Agent désenregistré
- `agent_status_changed` - Statut agent changé

## Configuration

### appsettings.json (AppHost)
```json
{
  "DnsConfiguration": {
    "Suffix": "pd3i1.com",
    "AppHostHost": "apphost",
    "OrchestratorHost": "orchestrator",
    "WorkerHostPrefix": "worker-agent",
    "VizHost": "viz",
    "Ports": {
      "AppHost": 5060,      // Registry API
      "Orchestrator": 5080,
      "Viz": 5070,
      "WorkerBase": 5090,
    "Dashboard": 18888
    },
    "WorkerCount": 3
  }
}
```

### Hosts File Entries
```
127.0.0.1 apphost.pd3i1.com
127.0.0.1 orchestrator.pd3i1.com
127.0.0.1 viz.pd3i1.com
127.0.0.1 worker-agent-1.pd3i1.com
127.0.0.1 worker-agent-2.pd3i1.com
127.0.0.1 worker-agent-3.pd3i1.com
127.0.0.1 dashboard.pd3i1.com
```

## Lifecycle Flow

### Démarrage Orchestrateur/Agent
1. Service démarre
2. Délai 2s pour stabilisation
3. **5 tentatives d'enregistrement** avec exponential backoff:
   - Attempt 1: immédiat
   - Attempt 2: +5s
   - Attempt 3: +10s
   - Attempt 4: +20s
   - Attempt 5: +40s
4. Si succès:
   - Reçoit ID unique
   - UpdateStatus("Running"/"Idle")
   - Démarre heartbeat loop (30s)
5. Si échec après 5 tentatives:
   - Log Warning
   - Continue en mode dégradé (sans registration)
 - Service reste fonctionnel

### Heartbeat
- Toutes les 30 secondes
- `POST /api/{orchestrators|agents}/{id}/heartbeat`
- Surveillance des échecs consécutifs (max 3)
- Après 3 échecs: tentative de re-registration automatique
- Pas d'événement WebSocket (évite spam)

### Changement de Statut
- `POST /api/{orchestrators|agents}/{id}/status` avec nouveau statut
- Registry met à jour le registre
- Diffuse `*_status_changed` via WebSocket
- UI met à jour l'affichage

### Shutdown Graceful
1. Signal shutdown reçu (Ctrl+C, StopAsync)
2. `StopAsync()` intercepte
3. UpdateStatus("Stopping") - best effort
4. UnregisterAsync() - best effort (pas d'échec si API down)
5. base.StopAsync() - arrête heartbeat loop
6. Dispose() - nettoie resources

## Testing Scenarios

### Scenario 1: Normal Operation
```bash
# Start AppHost (starts all services)
cd CSharp.ProjectDependency3/src/FrenchExDev.Net.CSharp.ProjectDependency3.AppHost
dotnet run
```
**Expected Logs**:
```
[Viz.Api] Registry API started on https://apphost.pd3i1.com:5060
[Orchestrator] Registration attempt 1/5
[Orchestrator] Orchestrator registered with ID: abc123
[Orchestrator] Orchestrator status updated to Running
[Agent-1] Registration attempt 1/5
[Agent-1] Agent registered with ID: def456 for orchestrator abc123
[Agent-1] Agent status updated to Idle
```

### Scenario 2: Registry API Unavailable at Startup
```bash
# Start services WITHOUT starting Viz.Api first
```
**Expected Behavior**:
- Services try 5 times with backoff (total ~75 seconds)
- After 5 failures: log Warning and continue
- Services remain functional in degraded mode
- No crash, no throw

**Expected Logs**:
```
[Orchestrator] Registration attempt 1/5
[Orchestrator] Network error during registration (no internet connection or DNS failure)
[Orchestrator] Registration attempt 1 failed. Retrying in 5 seconds...
[Orchestrator] Registration attempt 2/5
[Orchestrator] Network error during registration...
...
[Orchestrator] Failed to register after 5 attempts
[Orchestrator] Failed to register orchestrator after 5 attempts. Service will continue without registration.
```

### Scenario 3: Network Interruption During Runtime
```bash
# Disconnect network or stop Viz.Api after successful registration
```
**Expected Behavior**:
- First 3 heartbeat failures: logged as Warning
- After 3rd failure: automatic re-registration attempt
- If Registry returns: re-registers and continues
- If Registry stays down: continues without registration

**Expected Logs**:
```
[Orchestrator] Heartbeat network error (1/3)
[Orchestrator] Heartbeat network error (2/3)
[Orchestrator] Heartbeat network error (3/3)
[Orchestrator] Too many consecutive heartbeat failures. Attempting re-registration...
[Orchestrator] Registration attempt 1/5
...
```

### Scenario 4: Graceful Shutdown
```bash
# Press Ctrl+C
```
**Expected Logs**:
```
[Orchestrator] Orchestrator shutting down gracefully...
[Orchestrator] Orchestrator status updated to Stopping
[Orchestrator] Orchestrator unregistered successfully
```

### Scenario 5: Configuration Error
```json
// appsettings.json with MISSING RegistryApiUrl
{
  "RegistryApiUrl": ""
}
```
**Expected Logs**:
```
[Orchestrator] RegistryApiUrl not configured in appsettings
[Orchestrator] Failed to register after 5 attempts
[Orchestrator] Failed to register orchestrator after 5 attempts. Service will continue without registration.
```

## Troubleshooting

### Problem: Services can't register

**Symptom**: Logs show "Failed to register after 5 attempts"

**Possible Causes**:
1. **Registry API not running**
   - Solution: Start Viz.Api project
   - Check: `dotnet run --project FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api`

2. **Wrong RegistryApiUrl**
   - Check appsettings.json: `"RegistryApiUrl": "https://apphost.pd3i1.com:5060"`
   - Verify hosts file: `127.0.0.1 apphost.pd3i1.com`

3. **Firewall blocking port 5060**
   - Solution: Add firewall exception
   - Windows: `netsh advfirewall firewall add rule name="Aspire Registry" dir=in action=allow protocol=TCP localport=5060`

4. **Certificate issues**
   - Check cert files exist
   - Verify paths in configuration

### Problem: Heartbeats failing intermittently

**Symptom**: Logs show "Heartbeat network error (X/3)"

**Solution**: Normal behavior if network is unstable. Service will auto-recover after network restoration.

**Threshold**: Only after 3 consecutive failures, re-registration is attempted.

### Problem: UI doesn't show services

**Symptom**: `/lifecycle` page empty or shows "Disconnected"

**Possible Causes**:
1. **WebSocket not connecting**
   - Check browser console (F12)
   - Verify RegistryApiUrl in Viz configuration

2. **CORS issues**
   - Check Viz.Api Program.cs has `UseCors()`
   - Verify CORS policy allows Viz origin

3. **Services never registered**
   - Check orchestrator/agent logs for registration errors

## Future Enhancements

1. **Health Checks Integration**
   - Automatic status updates based on `/health` endpoints
   - Mark as Failed if health check fails

2. **Persistent Registry**
   - Database backing (SQLite/PostgreSQL)
   - Survive Registry API restarts

3. **Circuit Breaker Pattern**
   - Stop retries after prolonged failures
   - Resume after cooldown period

4. **Metrics & Monitoring**
   - Prometheus metrics export
   - Grafana dashboards
   - Alert on registration failures

5. **Dynamic Configuration**
   - Hot-reload of RegistryApiUrl
   - No restart needed for config changes

6. **Service Discovery**
   - Agents query Registry for available Orchestrators
   - Automatic load balancing

7. **Authentication**
   - JWT tokens for API calls
   - Prevent unauthorized registrations
