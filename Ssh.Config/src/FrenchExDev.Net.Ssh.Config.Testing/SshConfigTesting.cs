using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Ssh.Config.Testing;

/// <summary>
/// Provides helper classes for testing SSH configuration file parsing and validation logic.
/// </summary>
/// <remarks>This class contains nested types that facilitate unit testing of SSH configuration files and host
/// entries. The provided helpers allow callers to construct test configurations and assert expected outcomes,
/// streamlining the process of verifying correct parsing and error handling behaviors.</remarks>
public static class SshConfigTesting
{
    /// <summary>
    /// Provides helper methods for testing the validity of SSH configuration files using builder and assertion actions.
    /// </summary>
    /// <remarks>This class is intended for use in unit tests to streamline the process of constructing SSH
    /// configuration files and verifying their validity or failure cases. All methods are static and designed to be
    /// used without instantiating the class.</remarks>
    public static class FileTestter
    {
        /// <summary>
        /// Executes a test scenario by configuring an SSH config file and asserting its validity.
        /// </summary>
        /// <remarks>Use this method to encapsulate the setup and validation of SSH config file scenarios
        /// in unit tests. The <paramref name="assert"/> action is invoked only if the build succeeds.</remarks>
        /// <param name="body">An action that configures the <see cref="SshConfigFileBuilder"/> instance to define the SSH config file
        /// contents.</param>
        /// <param name="assert">An action that performs assertions on the result of building the SSH config file. Receives a <see
        /// cref="SshConfigFile"/> with a successful build.</param>
        public static void Valid(
            Action<SshConfigFileBuilder> body,
            Action<SshConfigFile> assert
        )
        {
            var builder = new SshConfigFileBuilder();
            body(builder);
            var file = builder.Build();
            assert(file.Success<SshConfigFile>());
        }

        /// <summary>
        /// Executes a test scenario in which building an SSH configuration file is expected to fail, and asserts the
        /// resulting failure.
        /// </summary>
        /// <remarks>Use this method to verify that invalid SSH configuration scenarios are correctly
        /// detected and handled by the builder. The method is intended for unit testing failure cases.</remarks>
        /// <param name="body">A delegate that configures the <see cref="SshConfigFileBuilder"/> to produce an invalid SSH configuration
        /// file.</param>
        /// <param name="assert">A delegate that performs assertions on the failure result produced by the builder.</param>
        public static void Invalid(
            Action<SshConfigFileBuilder> body,
            Action<FailuresDictionary> assert
        )
        {
            var builder = new SshConfigFileBuilder();
            body(builder);
            var file = builder.Build();
            assert(file.Failures<SshConfigFile>());
        }
    }

    /// <summary>
    /// Provides helper methods for testing the validity of SSH configuration host entries using builder and assertion
    /// delegates.
    /// </summary>
    /// <remarks>This class is intended for use in unit tests to verify the behavior of SSH configuration host
    /// objects. It enables callers to construct host entries with custom configurations and assert their validity or
    /// invalidity in a concise manner. All methods are static and thread-safe.</remarks>
    public static class HostEntryTester
    {
        /// <summary>
        /// Executes a test scenario by configuring an SSH host and asserting its validity.
        /// </summary>
        /// <remarks>Use this method to define and verify SSH host configuration scenarios in unit tests.
        /// The <paramref name="assert"/> action is invoked with the result of <see cref="SshConfigHost.Success"/> to
        /// facilitate validation.</remarks>
        /// <param name="body">An action that configures the SSH host using the provided <see cref="SshConfigHostBuilder"/> instance.</param>
        /// <param name="assert">An action that performs assertions on the result of the SSH host configuration. Receives a <see
        /// cref="SshConfigHost"/> instance representing the configured host.</param>
        public static void Valid(
            Action<SshConfigHostBuilder> body,
            Action<SshConfigHost> assert
        )
        {
            var builder = new SshConfigHostBuilder();
            body(builder);
            var host = builder.Build();
            assert(host.Success<SshConfigHost>());
        }

        /// <summary>
        /// Executes a test scenario in which an SSH configuration host build is expected to fail, and allows assertions
        /// on the resulting failure.
        /// </summary>
        /// <remarks>Use this method to verify that specific invalid configurations are correctly detected
        /// and reported as failures during SSH host configuration building.</remarks>
        /// <param name="body">An action that configures the <see cref="SshConfigHostBuilder"/> to produce an invalid or failing build
        /// result.</param>
        /// <param name="assert">An action that performs assertions on the <see cref="FailureObjectBuildResult{SshConfigHost,
        /// SshConfigHostBuilder}"/> produced by the failed build.</param>
        public static void Invalid(
            Action<SshConfigHostBuilder> body,
            Action<FailuresDictionary> assert
        )
        {
            var builder = new SshConfigHostBuilder();
            body(builder);
            var host = builder.Build();
            assert(host.Failures<SshConfigHost>());
        }
    }
}
