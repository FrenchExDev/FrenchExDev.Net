using FrenchExDev.Net.CSharp.Object.Result;
using System.Text;
using System.Text.Json;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Orchestrator.Services;

public interface IRegistrationService
{
    Task<Result<string>> RegisterAsync();
    Task UnregisterAsync();
    Task StartHeartbeatAsync(CancellationToken cancellationToken);
}

public class OrchestratorRegistrationService : BackgroundService, IRegistrationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrchestratorRegistrationService> _logger;
    private readonly HttpClient _httpClient;
    private Result<string> _registrationId = Result<string>.Failure();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private const int MaxRetryAttempts = 5;
    private const int InitialRetryDelaySeconds = 5;

    public OrchestratorRegistrationService(
        IConfiguration configuration,
        ILogger<OrchestratorRegistrationService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Wait a bit for the service to be fully ready
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

            // Try to register with retries
            _registrationId = await RegisterWithRetryAsync(stoppingToken);

            if (_registrationId.IsFailure)
            {
                _logger.LogWarning("Failed to register orchestrator after {MaxRetries} attempts. Service will continue without registration.", MaxRetryAttempts);
                // Continue running - don't throw, allow service to function in degraded mode
                return;
            }

            // Update status to Running
            await UpdateStatusAsync("Running", stoppingToken);

            // Start heartbeat loop
            await StartHeartbeatAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Registration service canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in registration service");
        }
    }

    private async Task<Result<string>> RegisterWithRetryAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        var delay = TimeSpan.FromSeconds(InitialRetryDelaySeconds);

        while (attempt < MaxRetryAttempts && !cancellationToken.IsCancellationRequested)
        {
            attempt++;
            _logger.LogInformation("Registration attempt {Attempt}/{MaxAttempts}", attempt, MaxRetryAttempts);

            var result = await RegisterAsync();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully registered on attempt {Attempt}", attempt);
                return result;
            }

            if (attempt < MaxRetryAttempts)
            {
                _logger.LogWarning("Registration attempt {Attempt} failed. Retrying in {Delay} seconds...", attempt, delay.TotalSeconds);

                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Registration retry canceled");
                    return Result<string>.Failure();
                }

                // Exponential backoff: 5s, 10s, 20s, 40s, 80s
                delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 120));
            }
        }

        _logger.LogError("Failed to register after {MaxAttempts} attempts", MaxRetryAttempts);
        return Result<string>.Failure();
    }

    public async Task<Result<string>> RegisterAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var apiUrl = _configuration["RegistryApiUrl"];
            if (string.IsNullOrEmpty(apiUrl))
            {
                _logger.LogError("RegistryApiUrl not configured in appsettings");
                return Result<string>.Failure(b => b.Add("Configuration", "RegistryApiUrl is missing"));
            }

            // Retrieve orchestrator URL from configuration
            var orchestratorUrl = _configuration["OrchestratorUrl"];
            if (string.IsNullOrEmpty(orchestratorUrl))
            {
                _logger.LogError("OrchestratorUrl not configured in appsettings");
                return Result<string>.Failure(b => b.Add("Configuration", "OrchestratorUrl is missing"));
            }

            var request = new { Url = orchestratorUrl };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage? response = null;
            try
            {
                response = await _httpClient.PostAsync($"{apiUrl}/api/orchestrators/register", content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Network error during registration (no internet connection or DNS failure)");
                return Result<string>.Failure(b => b.Add("Network", ex));
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Registration request timed out after {Timeout}s", _httpClient.Timeout.TotalSeconds);
                return Result<string>.Failure(b => b.Add("Timeout", ex));
            }

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RegistrationResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Id == null)
                {
                    _logger.LogError("Registration response is missing ID");
                    return Result<string>.Failure(b => b.Add("Response", "Missing ID in response"));
                }

                _registrationId = Result<string>.Success(result.Id);
                _logger.LogInformation("Orchestrator registered with ID: {Id} and URL: {Url}", result.Id, orchestratorUrl);
                return _registrationId;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError("Registration endpoint not found (404). Is RegistryApiUrl correct? {Url}", apiUrl);
                return Result<string>.Failure(b => b.Add("NotFound", $"Endpoint {apiUrl}/api/orchestrators/register not found"));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogWarning("Registry API is temporarily unavailable (503)");
                return Result<string>.Failure(b => b.Add("ServiceUnavailable", response));
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to register orchestrator: {StatusCode} - {Error}", response.StatusCode, errorBody);
                return Result<string>.Failure(b => b.Add("HttpError", $"{response.StatusCode}: {errorBody}"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration");
            return Result<string>.Failure(b => b.Add("Exception", ex));
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task UnregisterAsync()
    {
        if (_registrationId.IsFailure)
        {
            _logger.LogDebug("Skipping unregister - never successfully registered");
            return;
        }

        await _semaphore.WaitAsync();
        try
        {
            var apiUrl = _configuration["RegistryApiUrl"];
            if (string.IsNullOrEmpty(apiUrl))
            {
                _logger.LogWarning("Cannot unregister - RegistryApiUrl not configured");
                return;
            }

            try
            {
                await _httpClient.PostAsync($"{apiUrl}/api/orchestrators/{_registrationId.ObjectOrThrow()}/unregister", null);
                _logger.LogInformation("Orchestrator unregistered successfully");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Network error during unregister (non-critical)");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Unregister request timed out (non-critical)");
            }

            _registrationId = Result<string>.Failure();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering orchestrator");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task StartHeartbeatAsync(CancellationToken cancellationToken)
    {
        if (_registrationId.IsFailure)
        {
            _logger.LogDebug("Skipping heartbeat - not registered");
            return;
        }

        var apiUrl = _configuration["RegistryApiUrl"];
        if (string.IsNullOrEmpty(apiUrl))
        {
            _logger.LogWarning("Cannot send heartbeats - RegistryApiUrl not configured");
            return;
        }

        var consecutiveFailures = 0;
        const int maxConsecutiveFailures = 3;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    await _httpClient.PostAsync($"{apiUrl}/api/orchestrators/{_registrationId.ObjectOrThrow()}/heartbeat", null, cancellationToken);
                    _logger.LogDebug("Heartbeat sent for orchestrator {Id}", _registrationId.ObjectOrThrow());
                    consecutiveFailures = 0; // Reset on success
                }
                catch (HttpRequestException ex)
                {
                    consecutiveFailures++;
                    _logger.LogWarning(ex, "Heartbeat network error ({Failures}/{MaxFailures})", consecutiveFailures, maxConsecutiveFailures);

                    if (consecutiveFailures >= maxConsecutiveFailures)
                    {
                        _logger.LogError("Too many consecutive heartbeat failures. Attempting re-registration...");
                        _registrationId = await RegisterWithRetryAsync(cancellationToken);

                        if (_registrationId.IsSuccess)
                        {
                            consecutiveFailures = 0;
                            await UpdateStatusAsync("Running", cancellationToken);
                        }
                    }
                }
                catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    consecutiveFailures++;
                    _logger.LogWarning(ex, "Heartbeat timeout ({Failures}/{MaxFailures})", consecutiveFailures, maxConsecutiveFailures);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unexpected error sending heartbeat");
            }
        }
    }

    private async Task UpdateStatusAsync(string status, CancellationToken cancellationToken)
    {
        if (_registrationId.IsFailure)
        {
            _logger.LogDebug("Skipping status update - not registered");
            return;
        }

        try
        {
            var apiUrl = _configuration["RegistryApiUrl"];
            if (string.IsNullOrEmpty(apiUrl)) return;

            var json = JsonSerializer.Serialize(status);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync($"{apiUrl}/api/orchestrators/{_registrationId.ObjectOrThrow()}/status", content, cancellationToken);
            _logger.LogInformation("Orchestrator status updated to {Status}", status);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Network error updating status (non-critical)");
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Status update timeout (non-critical)");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error updating status");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Orchestrator shutting down gracefully...");

        // Update status to Stopping (best effort)
        await UpdateStatusAsync("Stopping", cancellationToken);

        // Unregister (best effort)
        await UnregisterAsync();

        await base.StopAsync(cancellationToken);
    }

    private class RegistrationResponse
    {
        public string? Id { get; set; }
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        _semaphore?.Dispose();
        base.Dispose();
    }
}
