# Guide de Logging Aspire AppHost

## Vue d'ensemble

L'AppHost Aspire est configuré pour afficher tous les logs de tous les projets orchestrés. Les logs sont disponibles à deux endroits :

1. **Console de l'AppHost** - Tous les logs en temps réel
2. **Dashboard Aspire** - Interface web avec logs par ressource

## Configuration du Logging

### Niveaux de Log

#### Production (`appsettings.json`)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.Hosting.Lifetime": "Information",
      "Aspire.Hosting.Dcp": "Information",
   "Aspire.Hosting": "Information",
      "System.Net.Http.HttpClient": "Information"
    }
}
}
```

#### Développement (`appsettings.Development.json`)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.Hosting.Lifetime": "Information",
      "Aspire.Hosting.Dcp": "Debug",
      "Aspire.Hosting": "Debug",
      "System.Net.Http.HttpClient": "Debug"
    }
  }
}
```

### Format des Logs

Les logs utilisent le format **Simple Console** avec :
- ? **Timestamp** : Format `HH:mm:ss`
- ? **Scopes inclus** : Pour tracer le contexte
- ? **Multi-ligne** : Meilleure lisibilité
- ? **Couleurs** (en développement) : Différenciation visuelle

## Accès aux Logs

### 1. Console de l'AppHost

Lancer l'AppHost :
```bash
cd CSharp.ProjectDependency3\src\FrenchExDev.Net.CSharp.ProjectDependency3.AppHost
dotnet run
```

Tous les logs des projets s'affichent dans la console avec le format :
```
14:23:45 info: registry-api[0]
      Application started. Press Ctrl+C to shut down.
14:23:46 info: orchestrator[0]
      Registration attempt 1/5
14:23:47 info: viz[0]
      Now listening on: https://viz.pd3i1.com:5070
```

### 2. Dashboard Aspire

Accédez au dashboard via :
```
https://dashboard.pd3i1.com:18888
```

**Fonctionnalités du Dashboard :**
- ?? Vue d'ensemble de toutes les ressources
- ?? Onglet "Console" pour chaque projet
- ?? Filtrage et recherche dans les logs
- ?? Métriques et traces
- ?? Vue des dépendances entre services

### 3. Logs par Projet

Chaque projet peut également afficher ses logs directement :

```bash
# API Registry
cd CSharp.ProjectDependency3\src\FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api
dotnet run

# Orchestrator
cd CSharp.ProjectDependency3\src\FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Orchestrator
dotnet run

# Viz
cd CSharp.ProjectDependency3\src\FrenchExDev.Net.CSharp.ProjectDependency3.Viz
dotnet run

# Worker Agent
cd CSharp.ProjectDependency3\src\FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Agent
dotnet run
```

## Variables d'Environnement de Logging

L'AppHost configure automatiquement ces variables pour chaque projet :

```csharp
.WithEnvironment("Logging__Console__FormatterName", "simple")
.WithEnvironment("Logging__Console__FormatterOptions__SingleLine", "false")
.WithEnvironment("Logging__Console__FormatterOptions__IncludeScopes", "true")
.WithEnvironment("Logging__Console__FormatterOptions__TimestampFormat", "HH:mm:ss ")
.WithEnvironment("Logging__LogLevel__Default", "Information")
.WithEnvironment("Logging__LogLevel__Microsoft.AspNetCore", "Information")
```

## Personnalisation du Logging par Projet

### Option 1 : Via appsettings.json du projet

Chaque projet peut avoir son propre `appsettings.json` :

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
   "YourNamespace": "Trace"
    }
  }
}
```

### Option 2 : Via l'AppHost

Modifiez `AppHost.cs` pour un projet spécifique :

```csharp
var api = builder.AddProject<FrenchExDev_Net_CSharp_ProjectDependency3_Viz_Api>("registry-api")
    // ... autres configurations ...
    .WithEnvironment("Logging__LogLevel__YourNamespace", "Trace");
```

## Filtrage des Logs

### Dans la Console

Utilisez les outils système :

**PowerShell (Windows)**
```powershell
# Filtrer par projet
dotnet run | Select-String "registry-api"

# Filtrer par niveau
dotnet run | Select-String "error|warn"

# Exporter vers fichier
dotnet run | Tee-Object -FilePath logs.txt
```

**Bash (Linux/Mac)**
```bash
# Filtrer par projet
dotnet run | grep "registry-api"

# Filtrer par niveau
dotnet run | grep -E "error|warn"

# Exporter vers fichier
dotnet run | tee logs.txt
```

### Dans le Dashboard

Le Dashboard Aspire offre :
- ?? Recherche en temps réel
- ??? Filtrage par niveau de log
- ?? Filtrage temporel
- ?? Export des logs

## Structure des Logs

### Format Standard

```
[Timestamp] [LogLevel]: [Category][EventId]
      [Message]
 [Scope Information]
```

### Exemple

```
14:23:45 info: registry-api[Microsoft.Hosting.Lifetime][0]
      Now listening on: https://api.pd3i1.com:5060
 => SpanId:abc123, TraceId:xyz789, ParentId:0
```

**Composants :**
- `14:23:45` - Timestamp
- `info` - Niveau de log
- `registry-api` - Nom de la ressource
- `Microsoft.Hosting.Lifetime` - Catégorie
- `[0]` - Event ID
- Message sur les lignes suivantes
- Scopes avec SpanId/TraceId pour le tracing distribué

## Troubleshooting

### Les logs ne s'affichent pas dans la console

1. **Vérifiez le niveau de log** dans `appsettings.json`
2. **Vérifiez que le projet log effectivement** :
   ```csharp
   logger.LogInformation("Test message");
   ```
3. **Redémarrez l'AppHost**

### Trop de logs

Réduisez le niveau de logging :

```json
{
"Logging": {
    "LogLevel": {
    "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  }
}
```

### Logs manquants pour un projet spécifique

Vérifiez que le projet utilise `ILogger` correctement :

```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;
 
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public void DoWork()
    {
        _logger.LogInformation("Doing work...");
    }
}
```

### Dashboard inaccessible

1. Vérifiez l'URL : `https://dashboard.pd3i1.com:18888`
2. Vérifiez que mkcert est installé et les certificats générés
3. Vérifiez le fichier hosts : `127.0.0.1 dashboard.pd3i1.com`

## Logs Structurés

Pour des logs plus riches, utilisez la syntaxe structurée :

```csharp
_logger.LogInformation(
    "User {UserId} performed action {Action} at {Timestamp}",
 userId,
    action,
    DateTime.UtcNow
);
```

Le Dashboard Aspire peut filtrer et rechercher sur ces propriétés structurées.

## Intégration avec OpenTelemetry

Aspire intègre automatiquement OpenTelemetry. Les logs incluent :
- **TraceId** : ID unique pour toute la chaîne de requêtes
- **SpanId** : ID unique pour l'opération courante
- **ParentId** : Lien avec l'opération parente

Cela permet de tracer une requête à travers tous les services.

## Meilleures Pratiques

1. ? **Utilisez des niveaux appropriés** :
   - `Trace` : Détails ultra-fins (boucles, etc.)
   - `Debug` : Informations de débogage
   - `Information` : Événements normaux
   - `Warning` : Situations anormales mais gérables
   - `Error` : Erreurs nécessitant attention
   - `Critical` : Erreurs fatales

2. ? **Logs structurés** plutôt que concaténation :
   ```csharp
   // ? Bon
   _logger.LogInformation("Processing order {OrderId}", orderId);
   
   // ? Mauvais
   _logger.LogInformation($"Processing order {orderId}");
   ```

3. ? **Contexte avec scopes** :
   ```csharp
   using (_logger.BeginScope("Order {OrderId}", orderId))
   {
       _logger.LogInformation("Processing...");
       _logger.LogInformation("Completed");
   }
   ```

4. ? **Ne loggez pas d'informations sensibles** :
   - Mots de passe
- Tokens
   - Données personnelles (PII)

## Performance

Le logging peut impacter les performances :

1. **Évitez le logging excessif** dans les boucles
2. **Utilisez les guards** pour logs coûteux :
   ```csharp
   if (_logger.IsEnabled(LogLevel.Debug))
   {
   _logger.LogDebug("Expensive operation: {Data}", ComputeExpensiveData());
   }
   ```

3. **Logging asynchrone** : Aspire le fait automatiquement

## Resources

- [Documentation Aspire](https://learn.microsoft.com/aspnet/core/aspire/)
- [Logging .NET](https://learn.microsoft.com/dotnet/core/extensions/logging)
- [OpenTelemetry](https://opentelemetry.io/)
- [Dashboard Aspire](https://learn.microsoft.com/aspnet/core/aspire/fundamentals/dashboard)
