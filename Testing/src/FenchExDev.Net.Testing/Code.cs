namespace FenchExDev.Net.Testing;

/// <summary>
/// Specifies the type of test to be performed, such as unit, functional, or integration testing.
/// </summary>
/// <remarks>Use this enumeration to indicate the scope and purpose of a test within a testing framework. The
/// values correspond to common testing categories: unit tests validate individual components, functional tests verify
/// specific features or behaviors, and integration tests assess interactions between multiple components.</remarks>
public enum TestKind
{
    Unit,
    Functional,
    Integration
}

public static class Kind
{
    public const string Test = "test-kind";
    public const string Unit = "Unit";
    public const string Functional = "Functional";
    public const string Integration = "Integration";
}

public static class Integration
{
    public const string Test = "integration-kind";
    public const string FakeIntegration = "FakeIntegration";
    public const string DomainIntegration = "DomainIntegration";
    public const string InfrastructureIntegration = "InfrastructureIntegration";
}

public static class Internet
{
    public const string Test = "internet";
    public const string Online = "online";
    public const string Offline = "offline";
}

/// <summary>
/// Specifies the type of integration used within the application, distinguishing between fake, domain, and
/// infrastructure integrations.
/// </summary>
/// <remarks>Use this enumeration to indicate the integration approach for components or services. This can help
/// clarify whether a dependency is a test double, part of the domain logic, or an external infrastructure
/// integration.</remarks>
public enum IntegrationKind
{
    /// <summary>
    /// Specifies that the associated resource or operation is isolated from others, ensuring independent execution or
    /// state.
    /// </summary>
    Isolated,

    /// <summary>
    /// Represents a placeholder integration used for testing or demonstration purposes.
    /// </summary>
    /// <remarks>This type is intended for scenarios where a real integration is not available or required. It
    /// can be used to simulate integration points during development or automated testing. This class does not provide
    /// actual connectivity or functionality beyond acting as a stub.</remarks>
    FakeIntegration,

    /// <summary>
    /// Represents the domain-specific operations and services.
    /// </summary>
    /// <remarks>Use this class to coordinate interactions with the application's core domain logic.</remarks>
    DomainIntegration,

    /// <summary>
    /// Provides functionality for integrating and coordinating operations across multiple application domains or system
    /// boundaries.
    /// </summary>
    /// <remarks>Use this class to facilitate communication, data exchange, or workflow between distinct
    /// domains, such as different services, platforms, or security contexts. The specific integration mechanisms and
    /// supported scenarios depend on the implementation and configuration of the class.</remarks>
    CrossDomainIntegration,

    /// <summary>
    /// Provides functionality for integrating with external infrastructure systems.
    /// </summary>
    /// <remarks>Use this class to facilitate communication and data exchange between the application and
    /// third-party infrastructure services. The specific integration capabilities depend on the configured providers
    /// and supported protocols.</remarks>
    InfrastructureIntegration,
}

/// <summary>
/// Represents the name of a feature, providing a strongly typed identifier for feature-related operations.
/// </summary>
/// <param name="Name">The name of the feature. Cannot be null or empty.</param>
public record FeatureName(string Name)
{
    public static FeatureName Unknown => new("Unknown");
}

/// <summary>
/// Specifies a feature, test kind, and optional integration context for a class or method to aid in test categorization
/// and filtering.
/// </summary>
/// <remarks>Apply this attribute to test classes or methods to indicate the feature being tested, the type of
/// test, and, if applicable, the integration context. Multiple instances of this attribute can be applied to the same
/// target to associate it with multiple features or test kinds. This attribute is typically used by test frameworks or
/// custom tooling to organize, filter, or report on tests by feature and integration type.</remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class FeatureAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the feature associated with this instance.
    /// </summary>
    public FeatureName Feature { get; }

    /// <summary>
    /// Gets the type of test represented by this instance.
    /// </summary>
    public TestKind Kind { get; }

    /// <summary>
    /// Gets the type of integration associated with the current instance, if specified.
    /// </summary>
    public IntegrationKind? IntegrationKind { get; }

    /// <summary>
    /// Initializes a new instance of the FeatureAttribute class with the specified feature name, test kind, and
    /// optional integration kind.
    /// </summary>
    /// <param name="feature">The name of the feature associated with the test. Cannot be null or empty.</param>
    /// <param name="kind">The type of test to which the feature applies. Defaults to TestKind.Unit if not specified.</param>
    /// <param name="integrationKind">The integration kind for the test, or null if the test does not involve integration.</param>
    public FeatureAttribute(string feature, TestKind kind = TestKind.Unit, IntegrationKind integrationKind = Testing.IntegrationKind.Isolated)
    {
        Feature = new FeatureName(feature);
        Kind = kind;
        IntegrationKind = integrationKind;
    }
}

/// <summary>
/// Provides a test fixture for creating instances of a command tester type used in packer bundler tests.
/// </summary>
/// <typeparam name="TCommandTester">The type of the command tester to instantiate. Must be a reference type with a public parameterless constructor.</typeparam>
public class TestsUsing<TCommandTester>
    where TCommandTester : class, new()
{
    public TCommandTester NewTester()
    {
        return new TCommandTester();
    }
}