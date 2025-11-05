# Architecture Gateway avec YARP pour ProjectDependency3

## ?? Vue d'ensemble

Cette solution utilise **YARP (Yet Another Reverse Proxy)** pour normaliser les URLs et fournir un point d'entrée unique pour tous les services.

## ??? Architecture

```
???????????????????????????????????????????????????????????????
?                    NAVIGATEUR / CLIENT                       ?
???????????????????????????????????????????????????????????????
                       ?
                       ? HTTPS (Port 443 standard)
                       ?
                       ?
???????????????????????????????????????????????????????????????
?               GATEWAY (YARP Reverse Proxy)                   ?
?               Port: 443 (ou configuré)                       ?
?               Certificat SSL: *.pd3i1.com                    ?
?                                                              ?
?  Routes basées sur le Host header:                          ?
?    ????????????????????????????????????????????????????    ?
?    ? api.pd3i1.com      ? localhost:5060 (API)        ?    ?
?    ? viz.pd3i1.com      ? localhost:5070 (Viz UI)     ?    ?
?    ? orchestrator.pd3i1.com ? localhost:5080 (Orch)   ?    ?
?    ? worker-1.pd3i1.com ? localhost:5090 (Worker 1)   ?    ?
?    ? worker-2.pd3i1.com ? localhost:5091 (Worker 2)   ?    ?
?    ????????????????????????????????????????????????????    ?
???????????????????????????????????????????????????????????????
                       ?
                       ? Service Discovery via Aspire
                       ?
     ?????????????????????????????????????????????????????
     ?                 ?                  ?              ?
     ?                 ?                  ?              ?
???????????     ????????????      ????????????   ????????????
?   API   ?     ?   VIZ    ?      ?   ORCH   ?   ? WORKERS  ?
?  :5060  ?     ?  :5070   ?      ?  :5080   ?   ? :509X    ?
?localhost?     ?localhost ?      ?localhost ?   ?localhost ?
???????????     ????????????      ????????????   ????????????
```

## ?? Avantages de cette architecture

### 1. **URLs Normalisées**
- ? Public : `https://api.pd3i1.com` (port 443 par défaut, pas besoin de spécifier)
- ? Interne : Services communiquent via `localhost:port`
- ? Pas de ports bizarres exposés au navigateur

### 2. **Certificat SSL Unique**
- ? Un seul certificat wildcard `*.pd3i1.com` sur le Gateway
- ? Services internes peuvent utiliser localhost ou leurs propres certificats
- ? Pas de gestion de certificat complexe par service

### 3. **Compatible Aspire**
- ? Services backend utilisent la configuration Aspire standard
- ? Pas de conflit avec DCP (Distributed Control Plane)
- ? Service Discovery fonctionne nativement
- ? Health checks, métriques, traces via Aspire Dashboard

### 4. **Extensibilité et Résilience**
- ? Load balancing automatique (multiple destinations)
- ? Retry policies configurables
- ? Circuit breakers
- ? Request/Response transformation
- ? Rate limiting

## ?? Structure des fichiers

```
CSharp.ProjectDependency3/
??? src/
?   ??? Gateway/                        # ? Nouveau projet
?   ?   ??? Program.cs                  # Configuration YARP
?   ?   ??? appsettings.json            # Routes YARP
?   ?   ??? *.csproj                    # Package YARP
?   ?
?   ??? Viz.Api/                        # Backend API
?   ?   ??? Program.cs                  # Écoute sur localhost:5060
?   ?
?   ??? Viz/                            # Blazor UI
?   ?   ??? Program.cs                  # Écoute sur localhost:5070
?   ?
?   ??? Worker.Orchestrator/            # Orchestrateur
?   ?   ??? Program.cs                  # Écoute sur localhost:5080
?   ?
?   ??? Worker.Agent/                   # Workers
?   ?   ??? Program.cs                  # Écoute sur localhost:509X
?   ?
?   ??? AppHost/                        # Aspire AppHost
?       ??? AppHost.cs                  # Configure tous les services + Gateway
?       ??? appsettings.Development.json # Configuration DNS
?       ??? DevAppHost.cs               # Setup DNS et certificats
?
??? Directory.Packages.props            # Version YARP centralisée
```

## ?? Configuration

### 1. **Directory.Packages.props**

```xml
<PackageVersion Include="Yarp.ReverseProxy" Version="2.2.0" />
```

### 2. **Gateway/appsettings.json**

```json
{
  "ReverseProxy": {
    "Routes": {
      "api-route": {
        "ClusterId": "api-cluster",
        "Match": {
          "Hosts": [ "api.pd3i1.com" ]
        }
      }
    },
    "Clusters": {
      "api-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "https://localhost:5060"
          }
        },
        "HttpClient": {
          "DangerousAcceptAnyServerCertificate": true
        }
      }
    }
  }
}
```

### 3. **AppHost/appsettings.Development.json**

```json
{
  "DnsConfiguration": {
    "Suffix": "pd3i1.com",
    "ApiHost": "api",
    "GatewayHost": "gateway",
    "Ports": {
      "Api": 5060,
      "Gateway": 443
    }
  }
}
```

## ?? Utilisation

### Démarrage

```powershell
# 1. Lancer Visual Studio en tant qu'Administrateur (requis pour modifier hosts file)

# 2. Définir AppHost comme projet de démarrage

# 3. F5 ou Ctrl+F5
```

### Accès aux services

```bash
# Gateway (entrée publique)
https://api.pd3i1.com/              # Via Gateway (port 443)
https://api.pd3i1.com/health        # Health check

# Services internes (dev/debug uniquement)
https://localhost:5060/             # API direct
https://localhost:5070/             # Viz direct

# Aspire Dashboard
https://dashboard.pd3i1.com:8443/
```

## ?? Configuration YARP Avancée

### Load Balancing

```json
"Clusters": {
  "api-cluster": {
    "LoadBalancingPolicy": "RoundRobin",
    "Destinations": {
      "api-1": { "Address": "https://localhost:5060" },
      "api-2": { "Address": "https://localhost:5061" }
    }
  }
}
```

### Health Checks

```json
"Clusters": {
  "api-cluster": {
    "HealthCheck": {
      "Active": {
        "Enabled": true,
        "Interval": "00:00:10",
        "Timeout": "00:00:05",
        "Path": "/health"
      }
    }
  }
}
```

### Request Transformation

```json
"Routes": {
  "api-route": {
    "ClusterId": "api-cluster",
    "Match": { "Hosts": [ "api.pd3i1.com" ] },
    "Transforms": [
      { "RequestHeader": "X-Forwarded-Host", "Set": "api.pd3i1.com" },
      { "RequestHeader": "X-Forwarded-Proto", "Set": "https" }
    ]
  }
}
```

### Rate Limiting

```json
"Routes": {
  "api-route": {
    "RateLimiterPolicy": "fixed-window",
    "Metadata": {
      "PermitLimit": "100",
      "Window": "00:01:00"
    }
  }
}
```

## ?? Troubleshooting

### Le Gateway ne démarre pas

```powershell
# Vérifier que le port 443 n'est pas utilisé
netstat -ano | findstr ":443"

# Si occupé, changer le port dans appsettings.json
"DnsConfiguration": {
  "Ports": {
    "Gateway": 8443  # Utiliser un autre port
  }
}
```

### Certificat SSL non valide

```powershell
# Régénérer les certificats
# Dans AppHost.cs, forcer la régénération
builder.EnsureSetup(dnsConfig, logger, forceCertificateRegeneration: true);

# Ou utiliser EnsureSelfSignedCertificateSetup au lieu de mkcert
```

### Service backend non accessible

```powershell
# Vérifier que le backend écoute
netstat -ano | findstr ":5060"

# Vérifier les logs dans Aspire Dashboard
https://dashboard.pd3i1.com:8443/

# Tester directement le backend
curl -k https://localhost:5060/health
```

### DNS ne résout pas

```powershell
# Vérifier le fichier hosts
Get-Content C:\Windows\System32\drivers\etc\hosts | Select-String "pd3i1.com"

# Doit contenir:
# 127.0.0.1 api.pd3i1.com
# 127.0.0.1 gateway.pd3i1.com
# etc.

# Flush DNS cache
ipconfig /flushdns
```

## ?? Ressources

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [mkcert](https://github.com/FiloSottile/mkcert)

## ?? Prochaines étapes

1. ? Gateway YARP configuré
2. ? Ajouter tous les services (Viz, Orchestrator, Workers)
3. ? Configurer le load balancing pour plusieurs instances
4. ? Ajouter les retry policies et circuit breakers
5. ? Configurer l'authentification/autorisation centralisée
6. ? Ajouter le rate limiting par client
