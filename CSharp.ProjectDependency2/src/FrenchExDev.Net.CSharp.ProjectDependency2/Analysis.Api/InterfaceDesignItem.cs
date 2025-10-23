namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public record InterfaceDesignItem(
    string Name,
    int MethodCount,
    int PropertyCount,
    int EventCount,
    int GenericArity,
    int InheritanceDepth,
    bool NameStartsWithI,
    bool TooLarge,
    bool OnlyProperties
);
