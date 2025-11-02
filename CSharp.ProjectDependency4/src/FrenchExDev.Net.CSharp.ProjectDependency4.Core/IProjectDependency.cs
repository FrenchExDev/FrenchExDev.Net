using Microsoft.CodeAnalysis.MSBuild;
using System.Text.RegularExpressions;

namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core;

/// <summary>
/// Represents a dependency of a project within a solution or build system.
/// </summary>
/// <remarks>Implementations of this interface define the contract for project dependencies, which may include
/// references to other projects, packages, or components required for building or running the project. This interface
/// is typically used by project management and build tools to model and resolve dependencies between
/// projects.</remarks>
public interface IProjectDependency
{

}
