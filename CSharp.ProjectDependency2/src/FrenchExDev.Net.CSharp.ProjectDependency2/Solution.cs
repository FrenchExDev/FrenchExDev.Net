namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Represents a wrapper for a Roslyn solution, providing access to project and workspace information for code analysis
/// and manipulation.
/// </summary>
/// <remarks>Use this class to interact with a Roslyn solution in scenarios such as code analysis, refactoring, or
/// workspace management. The underlying solution instance can be accessed and operated on using Roslyn APIs. This class
/// does not expose additional functionality beyond encapsulating the provided solution.</remarks>
public class Solution
{
    private Microsoft.CodeAnalysis.Solution _solution;

    /// <summary>
    /// Initializes a new instance of the Solution class using the specified Roslyn solution.
    /// </summary>
    /// <remarks>This constructor allows integration with the Microsoft.CodeAnalysis workspace model, enabling
    /// further analysis or manipulation of the provided solution. The Solution instance maintains a reference to the
    /// supplied solution for subsequent operations.</remarks>
    /// <param name="solution">The Roslyn solution to be wrapped and managed by this instance. Cannot be null.</param>
    public Solution(Microsoft.CodeAnalysis.Solution solution)
    {
        _solution = solution;
    }

    /// <summary>
    /// Gets a collection containing all projects in the current solution.
    /// </summary>
    public ICollection<Microsoft.CodeAnalysis.Project> Projects => _solution.Projects.ToList();
}
