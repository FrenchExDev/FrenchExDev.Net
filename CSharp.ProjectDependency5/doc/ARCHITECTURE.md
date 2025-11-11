# CSharp.ProjectDependency5 - Architecture Documentation

## Overview

`CSharp.ProjectDependency5` is a .NET 9 solution analyzer that visualizes and analyzes C# project dependencies using Roslyn and MSBuild. The system uses .NET Aspire for orchestration and provides a Blazor-based web interface for interactive dependency visualization.

## Solution Structure

### Projects

#### Core Projects

- **FrenchExDev.Net.CSharp.ProjectDependency5.Core**
  - Core domain models and analysis engine
  - MSBuild and Roslyn workspace integration
  - Project dependency analysis algorithms
  - Report generation framework
  - Graph model for visualization

#### Application Projects

- **FrenchExDev.Net.CSharp.ProjectDependency5.DevAppHost**
  - .NET Aspire AppHost for orchestrating services
  - Development environment setup and configuration
  - Certificate management for HTTPS

- **FrenchExDev.Net.CSharp.ProjectDependency5.Api**
  - REST API service (Web API project)
  - Orchestrates analysis requests
  - Manages communication between Viz2 and Worker3

- **FrenchExDev.Net.CSharp.ProjectDependency5.Viz2**
  - Blazor Web App with Server and WebAssembly modes
  - Interactive UI for dependency visualization
  - D3.js-based graph renderer
  - Solution selection and analysis triggering

- **FrenchExDev.Net.CSharp.ProjectDependency5.Viz2.Client**
  - Blazor WebAssembly client components
  - Client-side rendering logic

- **FrenchExDev.Net.CSharp.ProjectDependency5.Worker3**
  - Background worker service
  - Executes analysis tasks
  - Processes solution files using Roslyn
  - Generates markdown reports and graph data

## Architecture Diagrams

### System Overview - Aspire DevAppHost Sequence

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#fff','noteBorderColor':'#333','actorBkg':'#f4f4f4','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#f4f4f4','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#e8e8e8','sequenceNumberColor':'#fff','altLabelBkgColor':'#f4f4f4','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant Developer
    participant DevAppHost
    participant Aspire
    participant CertMgr as Certificate Manager
    participant API
    participant Viz2
    participant Worker3

    Developer->>DevAppHost: dotnet run
    DevAppHost->>DevAppHost: CreateBuilder(args)
    DevAppHost->>CertMgr: EnsureSetup()
    
    alt Certificate Setup Required
        rect rgb(230, 245, 255)
        Note over CertMgr,CertMgr: Certificate Configuration Phase
        CertMgr->>CertMgr: Generate/Load Dev Certificates
        CertMgr->>CertMgr: Configure HTTPS Endpoints
        CertMgr-->>DevAppHost: Setup Complete
        end
    end
    
    rect rgb(255, 245, 230)
    Note over DevAppHost,Aspire: Service Registration Phase
    DevAppHost->>Aspire: WithProjectInstance(api)
    Aspire-->>DevAppHost: API Resource Registered
    
    DevAppHost->>Aspire: WithProjectInstance(viz)
    Aspire-->>DevAppHost: Viz2 Resource Registered
    
    DevAppHost->>Aspire: WithProjectInstance(worker)
    Aspire-->>DevAppHost: Worker3 Resource Registered
    end
    
    rect rgb(245, 255, 230)
    Note over DevAppHost,Worker3: Application Startup Phase
    DevAppHost->>Aspire: Build()
    DevAppHost->>Aspire: RunAsync()
    
    Aspire->>API: Launch on configured port
    Aspire->>Viz2: Launch on configured port
    Aspire->>Worker3: Launch on configured port
    end
    
    Aspire-->>Developer: Dashboard URL (https://localhost:port)
    
    Note over Developer,Worker3: All services running with HTTPS dev certificates
```

### Key Actors Interaction - Analysis Workflow

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#fff','noteBorderColor':'#333','actorBkg':'#f4f4f4','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#f4f4f4','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#e8e8e8','sequenceNumberColor':'#fff','altLabelBkgColor':'#f4f4f4','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant User
    participant Viz2 as Viz2 (Blazor UI)
    participant API
    participant Worker3
    participant Roslyn as Roslyn/MSBuild
    participant FileSystem

    rect rgb(230, 245, 255)
    Note over User,API: Session Initialization Phase
    User->>Viz2: Navigate to Analysis Page
    Viz2->>API: POST /sessions/create
    API->>API: Generate Session ID
    API-->>Viz2: Session ID
    end

    rect rgb(255, 245, 230)
    Note over User,Viz2: Solution Selection Phase
    User->>Viz2: Select Solution Path
    Viz2->>API: POST /analyze
    Note over API: Create analysis job
    API->>Worker3: Queue Analysis Task
    API-->>Viz2: Job ID
    end

    rect rgb(245, 255, 230)
    Note over Worker3,FileSystem: Analysis Execution Phase
    Worker3->>FileSystem: Read Solution File (.sln)
    Worker3->>Roslyn: Open Solution
    Roslyn->>FileSystem: Load Project Files (.csproj)
    Roslyn->>Worker3: Solution Workspace
    
    Worker3->>Worker3: Initialize MSBuild
    Worker3->>Roslyn: Load Project References
    
    loop For Each Analyzer
        Worker3->>Worker3: Run Analyzer
        Note over Worker3: StructuralCouplingAnalyzer<br/>ClassicalCouplingAnalyzer<br/>DirectionalCouplingAnalyzer<br/>CodeGraphAnalyzer
    end
    end
    
    rect rgb(255, 235, 245)
    Note over Worker3,API: Report Generation Phase
    Worker3->>Worker3: Generate Reports
    Worker3->>Worker3: Create Markdown Document
    Worker3->>Worker3: Generate Graph JSON
    
    Worker3-->>API: Analysis Complete
    API-->>Viz2: Job Status: Completed
    end
    
    rect rgb(230, 255, 245)
    Note over Viz2,Worker3: Result Retrieval Phase
    Viz2->>API: GET /artifacts/markdown
    API->>Worker3: Fetch Markdown
    Worker3-->>API: Markdown Content
    API-->>Viz2: Markdown Report
    
    Viz2->>API: GET /artifacts/graph
    API->>Worker3: Fetch Graph JSON
    Worker3-->>API: Graph Data
    API-->>Viz2: Graph JSON
    
    Viz2->>Viz2: Render Graph (D3.js)
    Viz2->>User: Display Results
    end
```

### Component Interaction - Create Session & Analyze

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'fontFamily':'arial','fontSize':'14px','primaryColor':'#fff','primaryTextColor':'#000','primaryBorderColor':'#333','lineColor':'#333','secondaryColor':'#fff','tertiaryColor':'#fff','noteTextColor':'#000','noteBkgColor':'#fff','noteBorderColor':'#333','actorBkg':'#f4f4f4','actorBorder':'#333','actorTextColor':'#000','actorLineColor':'#333','signalColor':'#333','signalTextColor':'#fff','labelBoxBkgColor':'#f4f4f4','labelBoxBorderColor':'#333','labelTextColor':'#000','loopTextColor':'#000','activationBorderColor':'#333','activationBkgColor':'#e8e8e8','sequenceNumberColor':'#fff','altLabelBkgColor':'#f4f4f4','altLabelBorderColor':'#333'}}}%%
sequenceDiagram
    participant Viz2
    participant API
    participant Worker3
    participant SessionMgr as Session Manager
    participant AnalysisEngine

    rect rgb(230, 245, 255)
    Note over Viz2,SessionMgr: Session Creation Phase
    Viz2->>API: POST /sessions/create
    API->>SessionMgr: CreateSession()
    SessionMgr->>SessionMgr: Generate Unique Session ID
    SessionMgr->>SessionMgr: Initialize Session State
    SessionMgr-->>API: Session ID + Worker URL
    API-->>Viz2: {sessionId, workerUrl}
    end

    rect rgb(255, 245, 230)
    Note over Viz2,AnalysisEngine: Solution Selection Phase
    Viz2->>Worker3: GET /solutions
    Worker3->>Worker3: Scan Solutions Directory
    Worker3-->>Viz2: List of .sln files
    Viz2->>Viz2: User selects solution
    end

    rect rgb(245, 255, 230)
    Note over Viz2,AnalysisEngine: Analysis Execution Phase
    Viz2->>API: POST /analyze {solutionPath}
    API->>Worker3: POST /analyze {solutionPath}
    Worker3->>AnalysisEngine: InitiateAnalysis()
    
    AnalysisEngine->>AnalysisEngine: Open MSBuild Workspace
    AnalysisEngine->>AnalysisEngine: Load Solution
    
    par Run Analyzers in Parallel
        AnalysisEngine->>AnalysisEngine: StructuralCouplingAnalyzer
        AnalysisEngine->>AnalysisEngine: ClassicalCouplingAnalyzer
        AnalysisEngine->>AnalysisEngine: DirectionalCouplingAnalyzer
        AnalysisEngine->>AnalysisEngine: CodeGraphAnalyzer
    end
    
    AnalysisEngine->>AnalysisEngine: Aggregate Results
    AnalysisEngine->>AnalysisEngine: Generate Markdown
    AnalysisEngine->>AnalysisEngine: Generate Graph JSON
    AnalysisEngine-->>Worker3: Analysis Complete
    Worker3-->>API: {jobId, status: "completed"}
    API-->>Viz2: Job Completed
    end

    rect rgb(255, 235, 245)
    Note over Viz2,Worker3: Result Retrieval Phase
    Viz2->>API: GET /artifacts/markdown?sln={path}
    API->>Worker3: Proxy Request
    Worker3-->>API: Markdown Content
    API-->>Viz2: Markdown Content
    
    Viz2->>API: GET /artifacts/graph?sln={path}
    API->>Worker3: Proxy Request
    Worker3-->>API: Graph JSON
    API-->>Viz2: Graph JSON
    
    Viz2->>Viz2: Render Visualization
    end
```

## Core Components

### Analysis Engine

#### MSBuild Integration
- **MsBuildRegisteringService**: Registers MSBuild instance for solution loading
- **MsBuildWorkspace**: Roslyn workspace wrapper for MSBuild projects
- **DefaultProjectCollection**: Manages MSBuild project collection

#### Domain Models
- **Solution**: Represents a C# solution with projects collection
- **Project**: Represents a C# project with dependencies
- **IProjectDependency**: Abstraction for project dependencies

#### Analyzers
All analyzers implement `IProjectAnalyzer` interface:

1. **StructuralCouplingAnalyzer**
   - Analyzes structural project references
   - Builds dependency graph from .csproj files

2. **ClassicalCouplingAnalyzer**
   - Calculates afferent coupling (Ca)
   - Calculates efferent coupling (Ce)
   - Computes instability metric: I = Ce/(Ca+Ce)

3. **DirectionalCouplingAnalyzer**
   - Identifies directional dependency patterns
   - Detects circular dependencies

4. **CodeGraphAnalyzer**
   - Generates graph model for visualization
   - Creates nodes and edges for D3.js rendering

#### Report Generators
All generators implement `IReportGenerator<T>`:

- **StructuralCouplingReportGenerator**
- **ClassicalCouplingReportGenerator**
- **DirectionalCouplingReportGenerator**
- **CodeGraphReportGenerator**

### Markdown Generation

- **MarkdownDocument**: Container for markdown sections
- **MarkdownSection**: Individual report section with title and content

## Technology Stack

### Backend
- **.NET 9**: Target framework
- **Roslyn**: Code analysis and compilation APIs
- **MSBuild**: Project system integration
- **.NET Aspire**: Service orchestration and development

### Frontend
- **Blazor Server & WebAssembly**: UI framework
- **D3.js**: Graph visualization
- **Bootstrap**: UI styling

### Analysis
- **Microsoft.CodeAnalysis.CSharp.Workspaces**: Roslyn APIs
- **Microsoft.Build**: MSBuild evaluation
- **Microsoft.Build.Locator**: MSBuild instance location

## Development Workflow

### Running the Application

1. **Start DevAppHost**
   ```bash
   cd CSharp.ProjectDependency5/src/FrenchExDev.Net.CSharp.ProjectDependency5.DevAppHost
   dotnet run
   ```

2. **Aspire Dashboard**
   - Opens automatically at `https://localhost:[port]`
   - Shows all running services (API, Viz2, Worker3)
   - Monitors health checks and logs

3. **Access Viz2 UI**
   - Navigate to Viz2 URL from Aspire dashboard
   - Create session
   - Select solution
   - Run analysis
   - View results and graph

### Analysis Flow

1. **Session Creation**: User requests a new analysis session via Viz2
2. **Solution Selection**: User browses and selects a .sln file
3. **Analysis Trigger**: Viz2 sends analysis request to API
4. **Job Queuing**: API forwards request to Worker3
5. **MSBuild Loading**: Worker3 loads solution using Roslyn
6. **Analysis Execution**: Multiple analyzers process the solution
7. **Report Generation**: Markdown and graph artifacts are created
8. **Result Delivery**: Viz2 retrieves and displays results

## Key Features

### Analysis Capabilities
- Project dependency mapping
- Coupling metrics calculation
- Circular dependency detection
- Graph visualization
- Markdown report generation

### Visualization
- Interactive D3.js graph
- Node and edge styling
- Zoom and pan controls
- Search and filtering
- Theme switching (light/dark)

### Development Features
- HTTPS development certificates
- Aspire dashboard integration
- Health check endpoints
- Hot reload support
- Structured logging

## Configuration

### Aspire Configuration
```csharp
await DevAppHost.Default(
    builder: () => DistributedApplication.CreateBuilder(args),
    environment: "Development")
.EnsureSetup()
.WithProjectInstance(
    resourceBuilder: (builder) => builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency5_Api>("api"),
    name: "api"
)
.WithProjectInstance(
    resourceBuilder: (builder) => builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency5_Viz2>("viz"),
    name: "viz"
)
.WithProjectInstance(
    resourceBuilder: (builder) => builder.AddProject<Projects.FrenchExDev_Net_CSharp_ProjectDependency5_Worker3>("worker"),
    name: "worker"
)
.Build()
.RunAsync();
```

### Certificate Management
- Auto-generated development certificates
- HTTPS configuration for all services
- PEM format support
- Cross-platform compatibility

## Extensibility

### Adding New Analyzers

1. Implement `IProjectAnalyzer` interface
2. Add analyzer to `ProjectAnalyzerAggregator`
3. Create corresponding `IReportGenerator<T>`
4. Register in analysis pipeline

### Custom Report Formats

1. Implement `IReportGenerator<TResult>`
2. Process analysis results
3. Generate markdown sections
4. Integrate with markdown document

## Security Considerations

- HTTPS enforced for all communication
- Development certificates for local testing
- No authentication implemented (development only)
- Solution paths validated against configured root

## Future Enhancements

- Real-time WebSocket progress updates
- Multiple simultaneous analyses
- Export to various formats (PDF, HTML)
- Caching of analysis results
- Advanced filtering and querying
- Namespace-level analysis
- Type-level dependency tracking

---

**Version**: 1.0  
**Last Updated**: 2024  
**Target Framework**: .NET 9  
**Architecture Style**: Microservices with Aspire Orchestration
