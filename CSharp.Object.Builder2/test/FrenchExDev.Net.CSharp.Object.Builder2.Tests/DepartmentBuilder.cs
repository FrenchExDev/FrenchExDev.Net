namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class DepartmentBuilder : AbstractBuilder<Department>
{
    public string? Name { get; set; }
    public EmployeeBuilder? ManagerBuilder { get; set; }
    public BuilderList<Employee, EmployeeBuilder> Employees { get; } = [];

    protected override Department Instantiate() => new()
    {
        Name = Name!,
        Manager = ManagerBuilder?.Reference().ResolvedOrNull(),
        Employees = [.. Employees.AsReferenceList().AsEnumerable()]
    };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Name, nameof(Name), failures, n => new StringIsEmptyOrWhitespaceException(n));
        ManagerBuilder?.Validate(visitedCollector, failures);
        ValidateListInternal(Employees, nameof(Employees), visitedCollector, failures);
    }

    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        if (ManagerBuilder is not null)
            BuildChild(ManagerBuilder, visitedCollector);
        BuildList<EmployeeBuilder, Employee>(Employees, visitedCollector);
    }

    public DepartmentBuilder WithName(string name) { Name = name; return this; }
    public DepartmentBuilder WithManager(Action<EmployeeBuilder> configure)
    {
        ManagerBuilder = new EmployeeBuilder();
        configure(ManagerBuilder);
        return this;
    }
    public DepartmentBuilder WithEmployee(Action<EmployeeBuilder> configure) { Employees.New(configure); return this; }
}
