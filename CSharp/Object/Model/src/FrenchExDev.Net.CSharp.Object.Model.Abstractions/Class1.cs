namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;
public class Arguments : HashSet<Arguments.Argument>
{
    public record Argument
    {
        public string Name { get; init; }
        public string Type { get; init; }
        public Argument(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}

public interface INamespace
{
    List<INamespaceElement> Elements { get; set; }
}

public interface INamed
{
    public string Name { get; }
}

public interface INamespaceElement : INamed
{

}

public class NamespaceDeclaration : INamespaceElement
{
    public string Name { get; }

    public NamespaceDeclaration(string name)
    {
        Name = name;
    }

    public static NamespaceDeclaration New(string name)
    {
        return new NamespaceDeclaration(name);
    }
}

public class ClassDeclaration : INamespaceElement
{
    public enum Modifier
    {
        Internal,
        Private,
        Protected,
        Public,
        Static
    }

    public interface IClassMember : INamed
    {

    }

    public interface IMethodDeclaration : IClassMember
    {

    }

    public interface IPropertyDeclaration : IClassMember
    {
    }

    public class MethodDeclaration : IMethodDeclaration
    {
        public string Name { get; }
        public string ReturnType { get; set; }
        public Arguments Arguments { get; set; }
        public MethodDeclaration(string name, string returnType, Arguments arguments)
        {
            Name = name;
            ReturnType = returnType;
            Arguments = arguments;
        }
    }

    public class PropertyDeclaration : IPropertyDeclaration
    {
        public string Name { get; }

        public string Type { get; set; }
        public PropertyDeclaration(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }


    public string Name { get; }

    public ClassDeclaration(string name)
    {
        Name = name;
    }
}

public class InterfaceDeclaration : INamespaceElement
{
    public enum Modifier
    {
        Internal,
        Private,
        Protected,
        Public,
        Static
    }

    public interface IInterfaceMember : INamed
    {

    }

    public class MethodSignature : IInterfaceMember
    {
        public string Name { get; }
        public string ReturnType { get; set; }
        public Arguments Arguments { get; set; }
        public MethodSignature(string name, string returnType, Arguments arguments)
        {
            Name = name;
            ReturnType = returnType;
            Arguments = arguments;
        }
    }

    public class PropertySignature : IInterfaceMember
    {
        public string Name { get; }

        public string Type { get; set; }
        public PropertySignature(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }

    public string Name { get; }

    public IEnumerable<Modifier> Modifiers { get; }

    public IEnumerable<MethodSignature> Methods { get; }

    public IEnumerable<PropertySignature> Properties { get; }

    public InterfaceDeclaration(string name, IEnumerable<Modifier> modifiers, IEnumerable<MethodSignature> methods, IEnumerable<PropertySignature> properties)
    {
        Name = name;
        Modifiers = modifiers;
        Methods = methods;
        Properties = properties;
    }
}

