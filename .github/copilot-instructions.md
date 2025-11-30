**Monorepo Overview**
- Multi-solution .NET 9 monorepo; every top-level folder like `CSharp.Object.Result/` or `Dotnet.Project/` contains its own `.sln`, `src/`, `test/`, and documentation subtree.
- `FrenchExDev.Net.md` is an auto-generated inventory; use it to discover project owners, metrics, and dependency graphs before introducing new cross-solution references.

**Architecture Conventions**
- Projects follow a strict layering: `*.Abstractions` (contracts), `*.Infrastructure` (default impls), `*.Testing` (test helpers/fakes), then `test/*.Tests` (xUnit + Shouldly). Wire new code to the smallest layer needed.
- Many solutions expose orchestrators via fluent patterns (e.g., `CSharp.Object.Result/src/.../Code.cs` `Result<T>` pipeline, `FailureDictionaryBuilder`); extend using existing fluent APIs instead of ad-hoc helpers.
- Aspire-related apps live under `CSharp.Aspire.Dev/` and expect dependency injection wiring consistent with the abstractions defined beside them.

**Build & Tooling**
- Target SDK is locked via `global.json` (9.0.0, roll-forward latestMajor); never downgrade project TFMs.
- Restore tools and build from repo root with `dotnet tool restore` then `dotnet build FrenchExDev.Net.sln` (warnings are errors per `Directory.Builds.props`).
- Central package versions live in `Directory.Packages.props`; add/update dependencies there instead of editing individual csproj files.

**Testing Workflow**
- Preferred entry point is `_Scripts/Run-SolutionTests.ps1`; it discovers tests per solution, supports `-Include`/`-Exclude` filters, `-Parallel` with `-MaxParallel`, and coverage generation/serving (`-GenerateCoverageHtml -Serve`).
- For focused coverage, use `._Scripts/Run-TestProject.ps1 -ProjectPath <*.Tests.csproj>`; it handles coverlet collector vs msbuild and ReportGenerator output.
- Global test utilities (assert helpers, fixture builders) ship from `Testing/` and `FenchExDev.Net.Testing`; reference those before creating new test infrastructure.

**Documentation Expectations**
- Each solution maintains `doc/ARCHITECTURE.md` with the mermaid theme defined in `DOCUMENTATION-GUIDELINES.md`; reuse that init block and pastel phases when adding diagrams.
- Update the solution-specific README alongside code changes; follow the standardized sections in `DOCUMENTATION-GUIDELINES.md` (Overview, Quick Start, Workflow diagrams).

**Integration Patterns**
- `Dotnet.Project/**` defines MSBuild and project-type abstractions consumed by packer/mm solutions; extend these by adding new `*.Abstractions` contracts before wiring infrastructure providers.
- Packaging stacks (`Packer`, `Vos`, `Vagrant`, `VirtualBox.Version`) rely on deterministic version detection and YAML/JSON emitters—reuse existing serializers (see `Packer/src/.../Bundle`) rather than introducing new ones.
- HTTP/service integrations lean on `Microsoft.Extensions.*` + OpenTelemetry packages already referenced centrally; keep telemetry wiring consistent with existing implementation in `HttpClient/src` and Aspire projects.

**NuGet & Feeds**
- Local packages resolve via solution-level `NuGet.config` files pointing at `__Local_Nuget_Registry__`; keep those paths intact and publish new packages there when iterating locally.
- When consuming FrenchExDev packages, prefer project references within the monorepo during development; only switch to package references when mirroring released artifacts.

**Quality Gates**
- Treat analyzers seriously: `TreatWarningsAsErrors=true`, Roslynator and nullable are enforced; run `dotnet format` or VS Code analyzers before committing.
- Large generated artefacts (`FrenchExDev.Net.md`, coverage outputs under `test/report`) are machine-maintained—avoid manual edits and regenerate via scripts when necessary.
