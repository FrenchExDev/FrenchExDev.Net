# Service Discovery via Registry API

## Overview

Le système utilise maintenant une approche centralisée de service discovery via l'API Registry. Au lieu d'avoir des URLs hardcodées pour l'orchestrateur, tous les services interrogent dynamiquement l'API Registry pour découvrir les services disponibles.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│       Aspire AppHost                                        │
│  - Configure tous les services                              │
│  - Passe RegistryApiUrl à tous les composants               │
└────────────┬────────────────────────────────────────────────┘
           │
  ┌────────┴─────────┬──────────┬────────────┬─────────────┐
  │        │         │          │            │             │
┌───▼────────┐  ┌──────▼─────┐ ┌─▼──────┐ ┌──▼──────┐ ┌────▼────┐
│ Registry   │  │Orchestrator│ │  Viz   │ │ Agents  │ │Dashboard│
│    API     │  │            │ │   UI   │ │  1-N    │ │         │
│            │  │            │ │        │ │         │ │         │
│ - Register │◄─┤ Register   │ │Discover│◄┤Register │ │         │
│ - Discover │  │ Heartbeat  │ │Monitor │ │Heartbeat│ │         │
│ - WebSocket│  └────────────┘ └────────┘ └─────────┘ └─────────┘
└────────────┘
```

## Changements Implémentés

### 1. AnalysisService (Viz)

**Avant**:
```csharp
public AnalysisService(ISessionManager sessions, ILocalStorageService local, string orchestratorUrl)
{
  _orchestratorUrl = orchestratorUrl; // URL hardcodée
}
```

**Après**:
```csharp
public AnalysisService(ISessionManager sessions, ILocalStorageService local, string registryApiUrl)
{
    _registryApiUrl = registryApiUrl;
}

public async Task<AnalysisRunResult> AnalyzeSolutionAsync(string solutionPath, CancellationToken ct = default)
{
    // Découverte dynamique de l'orchestrateur
    var orchestratorUrl = await GetOrchestratorUrlAsync(ct);
    
    if (string.IsNullOrEmpty(orchestratorUrl))
    {
   return BuildErrorResult("No orchestrator available");
    }
    // ...
}

private async Task<string?> GetOrchestratorUrlAsync(CancellationToken ct)
{
    using var http = new HttpClient { BaseAddress = new Uri(_registryApiUrl) };
    var response = await http.GetAsync("/api/orchestrators", ct);
    
    if (!response.IsSuccessStatusCode)
   return null;

    var orchestrators = await response.Content.ReadFromJsonAsync<List<OrchestratorInfo>>(ct);
    
    // Retourne le premier orchestrateur en statut "Running"
    var running = orchestrators?.FirstOrDefault(o => o.Status == "Running");
    return running?.Url ?? orchestrators?.First().Url;
}
```

**Avantages**:
- ✅ Pas besoin de redémarrer le UI si l'orchestrateur change
- ✅ Load balancing automatique (peut choisir orchestrateur le moins chargé)
- ✅ Failover automatique si un orchestrateur tombe
- ✅ Découverte dynamique de nouveaux orchestrateurs

### 2. Program.cs (Viz)

**Avant**:
```csharp
var orchestratorUrl = builder.Configuration["OrchestratorUrl"] ?? "http://localhost:5080";

builder.Services.AddScoped<Services.IAnalysisService>(sp => 
  new Services.AnalysisService(
        sp.GetRequiredService<Services.ISessionManager>(),
        sp.GetRequiredService<ILocalStorageService>(),
        orchestratorUrl)); // URL directe
```

**Après**:
```csharp
var registryApiUrl = builder.Configuration["RegistryApiUrl"] ?? "https://api.pd3i1.com:5060";

builder.Services.AddScoped<Services.IAnalysisService>(sp => 
    new Services.AnalysisService(
        sp.GetRequiredService<Services.ISessionManager>(),
        sp.GetRequiredService<ILocalStorageService>(),
        registryApiUrl)); // URL de découverte
```

### 3. AppHost.cs

**Avant**:
```csharp
var viz = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz>("viz")
    .WithEnvironment("OrchestratorUrl", orchestratorUrl) // URL directe
    .WithReference(orchestrator);
```

**Après**:
```csharp
var apiUrl = dnsConfig.GetApiHostUrl();

var api = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz_Api>("registry-api")
    .WithHttpsEndpoint(port: dnsConfig.Ports.Api, name: "https");

var orchestrator = builder.AddProject<...>("orchestrator")
 .WithEnvironment("RegistryApiUrl", apiUrl) // Pour s'enregistrer
    .WithReference(api);

var viz = builder.AddProject<...>("viz")
    .WithEnvironment("RegistryApiUrl", apiUrl) // Pour découvrir
    .WithReference(api);

var worker = builder.AddProject<...>($"worker-agent-{i}")
 .WithEnvironment("RegistryApiUrl", apiUrl) // Pour s'enregistrer
    .WithReference(api);
```

**Variables d'environnement passées à tous les services**:
- `RegistryApiUrl` - URL de l'API Registry (ex: `https://api.pd3i1.com:5060`)

### 4. Configuration Files

**Nouveau fichier**: `wwwroot/appsettings.json`
```json
{
  "RegistryApiUrl": "https://api.pd3i1.com:5060"
}
```

**Nouveau fichier**: `wwwroot/appsettings.Development.json`
```json
{
  "RegistryApiUrl": "https://api.pd3i1.com:5060"
}
```

### 5. Lifecycle.razor

**Avant**:
```csharp
var appHostUrl = Configuration["AppHostUrl"] ?? "https://apphost.pd3i1.com:5060";
```

**Après**:
```csharp
var registryApiUrl = Configuration["RegistryApiUrl"] ?? "https://api.pd3i1.com:5060";
```

Affiche maintenant l'URL du Registry API dans l'interface.

## Configuration

### DnsConfiguration

```csharp
public class DnsConfiguration
{
    public string ApiHost { get; set; } = "api"; // Hostname pour Registry API
    
    public PortConfiguration Ports { get; set; } = new();
  
    public string GetApiHostFqdn() => $"{ApiHost}.{Domain}";
    public string GetApiHostUrl() => $"https://{GetApiHostFqdn()}:{Ports.Api}";
}

public class PortConfiguration
{
    public int Api { get; set; } = 5060; // Port pour Registry API
    public int Viz { get; set; } = 5070;
    public int Orchestrator { get; set; } = 5080;
    public int WorkerBase { get; set; } = 5090;
    public int Dashboard { get; set; } = 18888;
}
```

### Hosts File

Ajouter l'entrée pour l'API Registry:
```
127.0.0.1 api.pd3i1.com
```

## Workflow de Service Discovery

### Startup Sequence

1. **Registry API démarre** (port 5060)
   - Initialise les registres (vides)
   - Démarre le serveur WebSocket

2. **Orchestrator démarre**
   - Lit `RegistryApiUrl` depuis configuration
   - S'enregistre via `POST /api/orchestrators/register`
   - Reçoit un ID unique
   - Démarre heartbeat loop (30s)

3. **Agents démarrent**
   - Lisent `RegistryApiUrl`
   - Interrogent `/api/orchestrators` pour obtenir orchestrator ID
   - S'enregistrent via `POST /api/agents/register`
   - Démarrent heartbeat loop (30s)

4. **Viz UI démarre**
   - Lit `RegistryApiUrl` depuis `appsettings.json`
   - Utilisateur lance une analyse
   - UI interroge `/api/orchestrators` pour trouver orchestrateur disponible
   - Utilise l'URL découverte pour communiquer avec l'orchestrateur

### Runtime Flow

```
User → Viz UI: "Analyze solution X"
    ↓
Viz → Registry API: GET /api/orchestrators
    ↓
Registry API → Viz: [{ id: "abc", url: "https://orchestrator.pd3i1.com:5080", status: "Running" }]
    ↓
Viz → Orchestrator (discovered URL): POST /orchestrate/analyze
  ↓
Orchestrator → Agent: Delegate analysis task
    ↓
Agent → Orchestrator: Return results
    ↓
Orchestrator → Viz: Return analysis results
```

## Avantages de la Nouvelle Architecture

### 1. Dynamic Service Discovery
- ✅ Pas d'URLs hardcodées dans le code client
- ✅ Services peuvent être découverts automatiquement
- ✅ Support de multiples orchestrateurs (load balancing futur)

### 2. High Availability
- ✅ Si un orchestrateur tombe, UI peut en découvrir un autre
- ✅ Pas besoin de redémarrer les clients quand les services changent
- ✅ Health checks intégrés (status dans Registry)

### 3. Scalability
- ✅ Ajout/suppression dynamique d'orchestrateurs
- ✅ Agents peuvent découvrir dynamiquement leurs orchestrateurs
- ✅ Prêt pour déploiement cloud (Kubernetes, Docker Swarm)

### 4. Monitoring & Observability
- ✅ Vue centralisée de tous les services via Registry API
- ✅ Page `/lifecycle` pour monitoring en temps réel
- ✅ WebSocket pour notifications instantanées

### 5. Development Experience
- ✅ Configuration simplifiée (une seule URL: RegistryApiUrl)
- ✅ Pas de coordination manuelle des URLs entre services
- ✅ Facile à tester localement

## Testing

### Test 1: Service Discovery
```bash
# Démarrer Registry API
cd src/FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api
dotnet run

# Démarrer Orchestrator
cd src/FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Orchestrator
dotnet run

# Vérifier l'enregistrement
curl https://api.pd3i1.com:5060/api/orchestrators
# Devrait retourner l'orchestrateur enregistré

# Démarrer Viz UI
cd src/FrenchExDev.Net.CSharp.ProjectDependency3.Viz
dotnet run

# Ouvrir https://viz.pd3i1.com:5070
# Lancer une analyse
# → UI découvre automatiquement l'orchestrateur
```

### Test 2: Multiple Orchestrators
```bash
# Démarrer 2 orchestrateurs sur des ports différents
dotnet run --project Orchestrator --urls="https://orchestrator.pd3i1.com:5080"
dotnet run --project Orchestrator --urls="https://orchestrator.pd3i1.com:5081"

# Vérifier les deux enregistrements
curl https://api.pd3i1.com:5060/api/orchestrators
# Devrait retourner 2 orchestrateurs

# UI peut maintenant choisir entre les deux (round-robin, least-busy, etc.)
```

### Test 3: Failover
```bash
# Démarrer orchestrator
dotnet run --project Orchestrator

# Lancer analyse dans UI → fonctionne

# Arrêter orchestrator (Ctrl+C)

# Démarrer un nouveau orchestrator
dotnet run --project Orchestrator

# Lancer nouvelle analyse dans UI
# → Découvre automatiquement le nouvel orchestrator
```

## Migration Guide

Pour les services existants qui utilisent encore des URLs hardcodées:

### Étape 1: Ajouter RegistryApiUrl à la configuration
```json
{
  "RegistryApiUrl": "https://api.pd3i1.com:5060"
}
```

### Étape 2: Modifier le constructeur du service
```csharp
// Avant
public MyService(string orchestratorUrl)

// Après
public MyService(string registryApiUrl)
```

### Étape 3: Ajouter méthode de découverte
```csharp
private async Task<string?> GetOrchestratorUrlAsync()
{
    using var http = new HttpClient { BaseAddress = new Uri(_registryApiUrl) };
    var response = await http.GetAsync("/api/orchestrators");
    
    if (!response.IsSuccessStatusCode)
        return null;
        
    var orchestrators = await response.Content.ReadFromJsonAsync<List<OrchestratorInfo>>();
    return orchestrators?.FirstOrDefault(o => o.Status == "Running")?.Url;
}
```

### Étape 4: Utiliser la découverte avant chaque appel
```csharp
public async Task DoWorkAsync()
{
    var orchestratorUrl = await GetOrchestratorUrlAsync();
    
    if (string.IsNullOrEmpty(orchestratorUrl))
    {
    throw new Exception("No orchestrator available");
    }
    
    // Utiliser orchestratorUrl pour les appels
    using var http = new HttpClient { BaseAddress = new Uri(orchestratorUrl) };
    await http.PostAsync("/some-endpoint", content);
}
```

## Future Enhancements

1. **Load Balancing**
   - Choisir orchestrateur basé sur charge actuelle
   - Endpoint `/api/orchestrators/{id}/load` retournant charge CPU/memory

2. **Health Checks**
   - Intégration avec ASP.NET Core Health Checks
   - Status automatique basé sur `/health` endpoint

3. **Service Mesh Integration**
   - Support Istio, Linkerd pour discovery avancé
   - mTLS entre services

4. **Caching**
   - Cache local des URLs découvertes (TTL 30s)
   - Réduction des appels au Registry API

5. **Circuit Breaker**
   - Ne pas interroger Registry API si down
   - Fallback sur URLs en cache
