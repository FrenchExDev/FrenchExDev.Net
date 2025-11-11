# CSharp.Aspire.Dev - Architecture Documentation

## Overview

CSharp.Aspire.Dev is a .NET 9 development infrastructure library that simplifies local development environment setup for .NET Aspire distributed applications. It automates the configuration of HTTPS certificates, DNS entries, launch settings, and environment variables to enable consistent and secure local development experiences.

## Solution Structure

### Projects

#### Core Library Projects

- **FrenchExDev.Net.CSharp.Aspire.Dev**
  - Main library providing `DevAppHost` orchestration
  - DNS and certificate configuration management
  - Hosts file automation
  - Launch settings generation
  - Self-signed and mkcert certificate support

- **FrenchExDev.Net.CSharp.Aspire.App**
  - Launch settings helper utilities
  - JSON manipulation for launchSettings.json
  - Profile and environment variable management

- **FrenchExDev.Net.CSharp.Aspire.Dev.WebApplication**
  - ASP.NET Core integration extensions
  - Kestrel certificate configuration
  - WebApplicationBuilder extensions

- **FrenchExDev.Net.CSharp.Aspire.Dev.WebAssembly**
  - Blazor WebAssembly specific extensions
  - Client-side configuration helpers

## Architecture Diagrams

### DevAppHost Setup Sequence

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#ffffcc','noteBorderColor':'#333','actorBkg':'#fff','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#fff','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#f4f4f4','sequenceNumberColor':'#fff','altLabelBkgColor':'#fff','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant Dev as Developer
    participant Program as Program.cs
    participant DevAppHost
    participant DnsConfig as DNS Configuration
    participant CertMgr as Certificate Manager
    participant HostsFile as Hosts File
    participant LaunchSettings
    participant Aspire as .NET Aspire Builder

    Dev->>Program: dotnet run
    Program->>DevAppHost: Default(builder, "apphost", "devapphost.Development.json")
    DevAppHost->>DevAppHost: Load Configuration
    
    Program->>DevAppHost: EnsureSetup(force: false)
    
    rect rgb(220, 235, 255)
    Note over DevAppHost,CertMgr: Certificate Setup Phase
    DevAppHost->>DnsConfig: Load DnsConfiguration
    DevAppHost->>DevAppHost: NeedsCertificateRegeneration()
    
    alt Configuration Changed or Missing
        DevAppHost->>DevAppHost: LoadSavedConfiguration()
        DevAppHost->>DevAppHost: CalculateConfigurationHash()
    end
    
    alt Using mkcert
        DevAppHost->>CertMgr: EnsureCertSetupUsingMkCert()
        CertMgr->>CertMgr: Check mkcert installation
        CertMgr->>CertMgr: mkcert -install (CA)
        CertMgr->>CertMgr: Generate certificates for domains
        CertMgr->>DnsConfig: SetupCertAndKeyPaths()
    else Using C# Generated Certs
        DevAppHost->>CertMgr: EnsureCertSetupUsingGeneratedCerts()
        CertMgr->>CertMgr: Generate RSA self-signed certificate
        CertMgr->>CertMgr: Export to PEM format
        CertMgr->>CertMgr: Install to certificate store
        
        alt Windows with Admin
            CertMgr->>CertMgr: Add to LocalMachine\Root
        else Windows without Admin
            CertMgr->>CertMgr: Add to CurrentUser\Root
        else Linux
            CertMgr->>CertMgr: sudo cp to /usr/local/share/ca-certificates
            CertMgr->>CertMgr: sudo update-ca-certificates
        else macOS
            CertMgr->>CertMgr: sudo security add-trusted-cert
        end
        
        CertMgr->>DnsConfig: SetupCertAndKeyPaths()
    end
    
    DevAppHost->>DevAppHost: SaveConfiguration()
    end
    
    rect rgb(255, 245, 220)
    Note over DevAppHost,HostsFile: Hosts File Setup Phase
    DevAppHost->>DevAppHost: EnsureHostsSetup()
    DevAppHost->>DevAppHost: NeedsHostsFileUpdate()
    
    alt Updates Needed
        alt Windows with Admin
            DevAppHost->>HostsFile: Update C:\Windows\System32\drivers\etc\hosts
        else Unix Systems
            DevAppHost->>HostsFile: sudo append to /etc/hosts
        end
    end
    end
    
    rect rgb(220, 255, 235)
    Note over DevAppHost,LaunchSettings: Launch Settings Setup Phase
    DevAppHost->>DevAppHost: EnsureLaunchSettingsSetup()
    DevAppHost->>LaunchSettings: AddOrUpdateEnvironmentVariables()
    LaunchSettings->>LaunchSettings: Set ASPNETCORE_Kestrel__Certificates__*
    LaunchSettings->>LaunchSettings: Set ASPNETCORE_URLS
    LaunchSettings->>LaunchSettings: Set ASPIRE_* endpoints
    LaunchSettings->>LaunchSettings: Set CustomDomain__*
    end
    
    DevAppHost-->>Program: IDevAppHost
    
    rect rgb(255, 235, 245)
    Note over Program,Aspire: Project Registration Phase
    Program->>DevAppHost: WithProjectInstance(api, "api")
    DevAppHost->>Aspire: Configure API project resource
    DevAppHost->>Aspire: Set environment variables
    DevAppHost->>Aspire: Configure HTTPS URL
    
    Program->>DevAppHost: WithProjectInstance(viz, "viz")
    DevAppHost->>Aspire: Configure Viz project resource
    
    Program->>DevAppHost: WithProjectInstance(worker, "worker")
    DevAppHost->>Aspire: Configure Worker project resource
    end
    
    Program->>DevAppHost: Build()
    DevAppHost->>Aspire: Build distributed application
    Aspire-->>Program: DistributedApplication
    
    Program->>Aspire: RunAsync()
    Aspire->>Aspire: Start all registered projects
    Aspire-->>Dev: Dashboard URL + Service URLs
```

### Key Actors and Component Interaction

```mermaid
graph TB
    subgraph "Development Environment"
        Dev[Developer]
        IDE[IDE/Terminal]
    end
    
    subgraph "CSharp.Aspire.Dev Library"
        DevAppHost[DevAppHost<br/>Main Orchestrator]
        DnsConfig[DnsConfiguration<br/>Domain & Port Mapping]
        CertManager[Certificate Manager<br/>SSL Setup]
        LaunchHelper[LaunchSettingsHelper<br/>Config Manager]
    end
    
    subgraph "System Resources"
        HostsFile[Hosts File<br/>/etc/hosts or C:\Windows\System32\drivers\etc\hosts]
        CertStore[Certificate Store<br/>System Trust Store]
        LaunchSettings[launchSettings.json<br/>Project Configuration]
    end
    
    subgraph ".NET Aspire Infrastructure"
        AspireBuilder[DistributedApplicationBuilder<br/>Service Orchestrator]
        Dashboard[Aspire Dashboard<br/>Monitoring UI]
    end
    
    subgraph "Application Projects"
        API[API Service<br/>REST Endpoints]
        Viz2[Viz2 Service<br/>Blazor UI]
        Worker3[Worker3 Service<br/>Background Worker]
    end
    
    Dev -->|1. dotnet run| IDE
    IDE -->|2. Execute| DevAppHost
    
    DevAppHost -->|3. Load Config| DnsConfig
    DevAppHost -->|4. Setup Certificates| CertManager
    CertManager -->|5. Install| CertStore
    
    DevAppHost -->|6. Update DNS| HostsFile
    DevAppHost -->|7. Configure| LaunchHelper
    LaunchHelper -->|8. Write| LaunchSettings
    
    DevAppHost -->|9. Register Projects| AspireBuilder
    AspireBuilder -->|10. Start Services| API
    AspireBuilder -->|10. Start Services| Viz2
    AspireBuilder -->|10. Start Services| Worker3
    
    AspireBuilder -->|11. Launch| Dashboard
    Dashboard -.->|Monitor| API
    Dashboard -.->|Monitor| Viz2
    Dashboard -.->|Monitor| Worker3
    
    API -->|HTTPS| DnsConfig
    Viz2 -->|HTTPS| DnsConfig
    Worker3 -->|HTTPS| DnsConfig
    
    Viz2 -->|HTTP Requests| API
    API -->|Commands| Worker3
    
    style DevAppHost fill:#4a90e2,stroke:#2d5a8c,stroke-width:3px,color:#000
    style DnsConfig fill:#7cb342,stroke:#558b2f,stroke-width:2px,color:#000
    style CertManager fill:#fb8c00,stroke:#e65100,stroke-width:2px,color:#000
    style AspireBuilder fill:#ab47bc,stroke:#7b1fa2,stroke-width:2px,color:#000
```

### Certificate Generation Flow

```mermaid
flowchart TD
    Start([Start Certificate Setup]) --> CheckGenerator{SSL Generator?}
    
    CheckGenerator -->|mkcert| CheckMkcert[Check mkcert Installation]
    CheckGenerator -->|C#| CheckExisting[Check Existing Certs]
    
    CheckMkcert --> MkcertInstalled{Installed?}
    MkcertInstalled -->|No| ThrowError[Throw: Install mkcert]
    MkcertInstalled -->|Yes| InstallCA[mkcert -install CA]
    
    InstallCA --> NeedsRegen1{Regeneration<br/>Required?}
    NeedsRegen1 -->|No| UseExisting1[Use Existing PEM Files]
    NeedsRegen1 -->|Yes| GenMkcert[Generate mkcert Certificates]
    GenMkcert --> SavePEM1[Save .pem and -key.pem Files]
    SavePEM1 --> SaveConfig1[Save Configuration Hash]
    
    CheckExisting --> NeedsRegen2{Regeneration<br/>Required?}
    NeedsRegen2 -->|No| UseExisting2[Use Existing PEM Files]
    NeedsRegen2 -->|Yes| GenRSA[Generate RSA 2048 Key]
    
    GenRSA --> CreateCSR[Create Certificate Request]
    CreateCSR --> AddSAN[Add Subject Alternative Names]
    AddSAN --> SelfSign[Create Self-Signed Certificate]
    SelfSign --> ExportPEM[Export to PEM Format]
    ExportPEM --> ExportPFX[Export to PFX Format]
    
    ExportPFX --> InstallStore{Platform?}
    InstallStore -->|Windows| CheckAdmin{Administrator?}
    CheckAdmin -->|Yes| InstallLM[Install to LocalMachine\Root]
    CheckAdmin -->|No| InstallCU[Install to CurrentUser\Root]
    
    InstallStore -->|Linux| InstallLinux[sudo update-ca-certificates]
    InstallStore -->|macOS| InstallMac[sudo security add-trusted-cert]
    
    InstallLM --> SavePEM2[Save PEM Files]
    InstallCU --> SavePEM2
    InstallLinux --> SavePEM2
    InstallMac --> SavePEM2
    
    SavePEM2 --> SaveConfig2[Save Configuration Hash]
    SaveConfig1 --> SetPaths[SetupCertAndKeyPaths]
    SaveConfig2 --> SetPaths
    UseExisting1 --> SetPaths
    UseExisting2 --> SetPaths
    
    SetPaths --> End([Certificate Setup Complete])
    
    style Start fill:#4caf50,stroke:#2e7d32,stroke-width:2px,color:#000
    style End fill:#4caf50,stroke:#2e7d32,stroke-width:2px,color:#000
    style ThrowError fill:#f44336,stroke:#c62828,stroke-width:2px,color:#000
    style GenMkcert fill:#2196f3,stroke:#1565c0,stroke-width:2px,color:#000
    style SelfSign fill:#2196f3,stroke:#1565c0,stroke-width:2px,color:#000
```

## Core Components

### DevAppHost

The central orchestration class that manages the complete development environment setup.

**Key Responsibilities:**
- Load and validate DNS configuration from JSON files
- Manage SSL certificate generation and installation
- Update system hosts file with development domain entries
- Configure launch settings with environment variables
- Register project resources with Aspire builder
- Build and return configured distributed application

**Key Methods:**
- `Default()`: Factory method to create DevAppHost with configuration
- `EnsureSetup()`: Main entry point for environment setup
- `EnsureCertSetup()`: Orchestrates certificate generation
- `EnsureHostsSetup()`: Updates hosts file
- `EnsureLaunchSettingsSetup()`: Configures launch settings
- `WithProjectInstance()`: Registers project resources
- `Build()`: Builds the distributed application

### DnsConfiguration

Configuration record that holds all DNS and certificate-related settings.

**Key Properties:**
- `Domain`: Base domain (e.g., "dev.local")
- `Domains`: Array of subdomains (e.g., ["api", "viz", "worker"])
- `Ports`: Dictionary mapping domain names to port numbers
- `AspNetCoreUrls`: URL templates for ASP.NET Core endpoints
- `SslGenerator`: Certificate generator to use ("C#" or "mkcert")
- `CertificatesDirectory`: Directory for storing certificates
- `CertPath` / `KeyPath`: Paths to certificate files

**Key Methods:**
- `GetUrl()`: Constructs HTTPS URLs for domains
- `GetAspNetCoreUrl()`: Generates ASP.NET Core URL from template
- `GetAllHosts()`: Returns all fully qualified host names
- `GetHostsFileEntries()`: Generates hosts file entries
- `CalculateConfigurationHash()`: Computes SHA256 hash for change detection

### Certificate Management

#### Self-Signed Certificates (C#)
1. Generate RSA 2048-bit key pair
2. Create X509 certificate request with SAN entries
3. Self-sign certificate with 2-year validity
4. Export to PEM format (certificate and private key)
5. Export to PFX with password
6. Install to system certificate store
7. Platform-specific trust store installation

#### mkcert Certificates
1. Check mkcert installation
2. Install local CA (`mkcert -install`)
3. Generate certificates for all domains
4. Save as PEM files
5. System automatically trusts mkcert CA

### Launch Settings Management

The `LaunchSettingsHelper` class provides utilities for manipulating `launchSettings.json`:

**Environment Variables Set:**
- `ASPNETCORE_Kestrel__Certificates__Default__Path`
- `ASPNETCORE_Kestrel__Certificates__Default__KeyPath`
- `ASPNETCORE_URLS`
- `ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL`
- `ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL`
- `ASPIRE_ENVIRONMENT`
- `DOTNET_ENVIRONMENT`
- `CustomDomain__Fqdn`
- `CustomDomain__Port`

### WebApplication Extensions

The `EnsureCertificateSetup()` extension method for `WebApplicationBuilder`:
- Reads certificate paths from configuration
- Loads PEM certificate and key
- Converts to X509Certificate2 with exportable private key
- Configures Kestrel HTTPS defaults
- Supports TLS 1.2 and TLS 1.3
- Does not override Aspire endpoint configuration

## Configuration Structure

### devapphost.json (Base Configuration)

```json
{
  "Environment": "Development",
  "LaunchSettingsProfile": "local-dev-https"
}
```

### devapphost.Development.json

```json
{
  "DnsConfiguration": {
    "DashboardAppName": "devdash",
    "SslPassword": "",
    "SslGenerator": "mkcert",
    "CertificatesDirectory": ".dev-certs",
    "Domain": "dev.local",
    "Domains": ["api", "viz", "worker", "devdash"],
    "Ports": {
      "api": 5000,
      "viz": 5001,
      "worker": 5002,
      "devdash": 15888,
      "devdash-oltp": 16666,
      "devdash-services": 17777
    },
    "AspNetCoreUrls": {
      "api": "https://#subdomain#.#domain#:#port#",
      "viz": "https://#subdomain#.#domain#:#port#",
      "worker": "https://#subdomain#.#domain#:#port#",
      "devdash": "https://#subdomain#.#domain#:#port#"
    }
  }
}
```

## Technology Stack

### Framework
- **.NET 9**: Target framework for all projects
- **.NET Aspire**: Distributed application orchestration

### Libraries
- **Newtonsoft.Json**: JSON manipulation for launch settings
- **System.Security.Cryptography**: Certificate generation
- **System.Security.Cryptography.X509Certificates**: Certificate store management

### External Tools (Optional)
- **mkcert**: Local CA and certificate generation (alternative to C# generated certs)

## Platform Support

### Windows
- Administrator privileges for LocalMachine certificate store
- Non-admin users can use CurrentUser store
- Automatic hosts file updates with admin rights
- Support for both mkcert and C# generated certificates

### Linux
- `update-ca-certificates` (Debian/Ubuntu)
- `update-ca-trust` (RHEL/CentOS)
- `sudo` required for hosts file updates
- Certificate installation to `/usr/local/share/ca-certificates/`

### macOS
- `security` CLI for keychain management
- `sudo` required for system keychain and hosts file
- Certificate installation to System keychain

## Development Workflow

### 1. Initial Setup

```csharp
var devAppHost = DevAppHost.Default(
    builder: () => DistributedApplication.CreateBuilder(args),
    loggerName: "apphost",
    jsonFile: "devapphost.#Env#.json",
    environment: "Development"
);
```

### 2. Environment Setup

```csharp
devAppHost.EnsureSetup(force: false);
```

This performs:
- Certificate generation/validation
- Hosts file updates
- Launch settings configuration

### 3. Project Registration

```csharp
devAppHost
    .WithProjectInstance(
        resourceBuilder: (builder) => builder.AddProject<Projects.Api>("api"),
        name: "api"
    )
    .WithProjectInstance(
        resourceBuilder: (builder) => builder.AddProject<Projects.Viz2>("viz"),
        name: "viz"
    )
    .WithProjectInstance(
        resourceBuilder: (builder) => builder.AddProject<Projects.Worker3>("worker"),
        name: "worker"
    );
```

### 4. Build and Run

```csharp
await devAppHost
    .Build()
    .RunAsync();
```

## Security Considerations

### Certificate Security
- Self-signed certificates for development only
- 2048-bit RSA keys
- 2-year validity period
- PFX files password-protected
- Private keys exportable for system use

### System Modifications
- Requires elevated privileges for:
  - Hosts file modifications
  - System certificate store access
- Changes are logged for traceability
- Configuration hash tracking prevents unnecessary regeneration

### Production Usage
**Warning:** This library is designed for development environments only. Do not use in production:
- Self-signed certificates are not trusted by external clients
- Hosts file modifications affect system-wide DNS
- Certificate stores are modified globally

## Extensibility

### Custom Certificate Generators

Implement custom certificate generation by:
1. Adding new `SslGenerator` value to configuration
2. Implementing generator method in `DevAppHost`
3. Ensuring PEM format output

### Custom DNS Configuration

Extend `DnsConfiguration` with:
- Additional domain patterns
- Custom port assignment logic
- Alternative URL templates

### Custom Project Configuration

Use `WithProjectInstance` configuration parameter:
```csharp
.WithProjectInstance(
    resourceBuilder: (b) => b.AddProject<Projects.Api>("api"),
    name: "api",
    configuration: (resource) => {
        resource.WithEnvironment("CUSTOM_VAR", "value");
        resource.WithReplicas(2);
    }
)
```

## Troubleshooting

### Certificate Issues
- **Not Trusted**: Ensure CA is installed (mkcert) or certificate is in system store
- **Permission Denied**: Run with administrator/sudo privileges
- **Already Exists**: Use `force: true` to regenerate

### Hosts File Issues
- **Updates Not Applied**: Check administrator privileges
- **DNS Not Resolving**: Verify hosts file syntax and flush DNS cache
- **Linux/macOS**: Ensure `sudo` access

### Launch Settings Issues
- **Variables Not Applied**: Check profile name matches configuration
- **File Not Found**: Verify launchSettings.json path
- **Invalid JSON**: Review launch settings file syntax

## Best Practices

1. **Use mkcert for Teams**: Ensures consistent certificate generation across team members
2. **Version Control**: Exclude `.dev-certs/` directory and `dns-config.json`
3. **Configuration**: Use environment-specific configuration files
4. **Domain Names**: Use `.local` or `.test` TLDs for development
5. **Port Assignment**: Use consistent port ranges (5000-5999 for dev)
6. **Documentation**: Document custom domains and ports for team

## Future Enhancements

- Docker container certificate mounting
- Kubernetes local development support
- Multiple environment profiles
- Certificate expiration monitoring
- Automatic DNS cache flushing
- Integration with other development tools
- GUI for configuration management

---

**Version**: 1.0  
**Last Updated**: 2024  
**Target Framework**: .NET 9  
**Architecture Style**: Development Infrastructure Library
