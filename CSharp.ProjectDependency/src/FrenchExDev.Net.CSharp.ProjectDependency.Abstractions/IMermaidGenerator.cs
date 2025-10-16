using System;
using System.Collections.Generic;
using System.Linq;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public interface IMermaidGenerator
{
    string Generate(SolutionAnalysis analysis);
}
