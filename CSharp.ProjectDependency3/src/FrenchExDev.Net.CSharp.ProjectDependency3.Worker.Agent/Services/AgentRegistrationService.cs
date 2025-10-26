using System.Text;
using System.Text.Json;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Worker.Agent.Services;

public interface IAgentRegistrationService
{
    Task<string?> RegisterAsync();
    Task UnregisterAsync();
    Task StartHeartbeatAsync(CancellationToken cancellationToken);
}

public class AgentRegistrationService : BackgroundService, IAgentRegistrationService
{
    private readonly IConfiguration _configuration;
  private readonly ILogger<AgentRegistrationService> _logger;
    private readonly HttpClient _httpClient;
    private string? _registrationId;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
  private const int MaxRetryAttempts = 5;
    private const int InitialRetryDelaySeconds = 5;

    public AgentRegistrationService(
   IConfiguration configuration,
  ILogger<AgentRegistrationService> logger,
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

      if (string.IsNullOrEmpty(_registrationId))
   {
                _logger.LogWarning("Failed to register agent after {MaxRetries} attempts. Service will continue without registration.", MaxRetryAttempts);
    // Continue running - don't throw, allow service to function in degraded mode
 return;
       }

  // Update status to Idle
    await UpdateStatusAsync("Idle", stoppingToken);

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

 private async Task<string?> RegisterWithRetryAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
   var delay = TimeSpan.FromSeconds(InitialRetryDelaySeconds);

      while (attempt < MaxRetryAttempts && !cancellationToken.IsCancellationRequested)
  {
  attempt++;
      _logger.LogInformation("Registration attempt {Attempt}/{MaxAttempts}", attempt, MaxRetryAttempts);

   var result = await RegisterAsync();

     if (!string.IsNullOrEmpty(result))
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
  return null;
  }

      // Exponential backoff: 5s, 10s, 20s, 40s, 80s
         delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 120));
  }
   }

     _logger.LogError("Failed to register after {MaxAttempts} attempts", MaxRetryAttempts);
  return null;
 }

    public async Task<string?> RegisterAsync()
    {
   await _semaphore.WaitAsync();
     try
        {
  var apiUrl = _configuration["RegistryApiUrl"];
   if (string.IsNullOrEmpty(apiUrl))
    {
     _logger.LogError("RegistryApiUrl not configured in appsettings");
 return null;
      }

       // Get orchestrator ID
var orchestratorId = await GetOrchestratorIdAsync();
    if (string.IsNullOrEmpty(orchestratorId))
{
      _logger.LogWarning("Could not obtain orchestrator ID, using default");
   orchestratorId = "default-orchestrator";
     }

      var agentUrl = _configuration["ASPNETCORE_URLS"]?.Split(';').FirstOrDefault();
      if (string.IsNullOrEmpty(agentUrl))
   {
              _logger.LogError("ASPNETCORE_URLS not configured");
      return null;
       }

  var request = new { OrchestratorId = orchestratorId, Url = agentUrl };
    var json = JsonSerializer.Serialize(request);
  var content = new StringContent(json, Encoding.UTF8, "application/json");

HttpResponseMessage? response = null;
      try
    {
    response = await _httpClient.PostAsync($"{apiUrl}/api/agents/register", content);
   }
         catch (HttpRequestException ex)
   {
      _logger.LogWarning(ex, "Network error during registration (no internet connection or DNS failure)");
           return null;
}
       catch (TaskCanceledException ex)
      {
   _logger.LogWarning(ex, "Registration request timed out after {Timeout}s", _httpClient.Timeout.TotalSeconds);
 return null;
  }

      if (response.IsSuccessStatusCode)
  {
     var responseJson = await response.Content.ReadAsStringAsync();
     var result = JsonSerializer.Deserialize<RegistrationResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (string.IsNullOrEmpty(result?.Id))
    {
   _logger.LogError("Registration response is missing ID");
    return null;
        }

            _registrationId = result.Id;
    _logger.LogInformation("Agent registered with ID: {Id} for orchestrator {OrchestratorId}", result.Id, orchestratorId);
        return _registrationId;
          }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
        _logger.LogError("Registration endpoint not found (404). Is RegistryApiUrl correct? {Url}", apiUrl);
        return null;
   }
  else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
    {
     _logger.LogWarning("Registry API is temporarily unavailable (503)");
       return null;
  }
            else
       {
  var errorBody = await response.Content.ReadAsStringAsync();
  _logger.LogError("Failed to register agent: {StatusCode} - {Error}", response.StatusCode, errorBody);
      return null;
        }
  }
   catch (Exception ex)
{
_logger.LogError(ex, "Unexpected error during registration");
    return null;
  }
        finally
   {
     _semaphore.Release();
    }
    }

    private async Task<string?> GetOrchestratorIdAsync()
    {
     try
{
      var apiUrl = _configuration["RegistryApiUrl"];
     if (string.IsNullOrEmpty(apiUrl))
   {
   _logger.LogDebug("RegistryApiUrl not configured, cannot query for orchestrator ID");
   return "default-orchestrator";
    }

       try
      {
        var response = await _httpClient.GetAsync($"{apiUrl}/api/orchestrators");
         if (response.IsSuccessStatusCode)
      {
      var json = await response.Content.ReadAsStringAsync();
var orchestrators = JsonSerializer.Deserialize<List<OrchestratorInfo>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

      if (orchestrators != null && orchestrators.Count > 0)
      {
  var first = orchestrators.First();
      _logger.LogInformation("Found orchestrator ID: {Id}", first.Id);
         return first.Id;
          }
     }
    else
  {
  _logger.LogDebug("Could not fetch orchestrators list: {StatusCode}", response.StatusCode);
      }
   }
 catch (HttpRequestException ex)
       {
     _logger.LogDebug(ex, "Network error fetching orchestrator ID");
        }
       catch (TaskCanceledException ex)
  {
  _logger.LogDebug(ex, "Timeout fetching orchestrator ID");
   }

    return "default-orchestrator";
     }
        catch (Exception ex)
 {
      _logger.LogWarning(ex, "Error getting orchestrator ID, using default");
      return "default-orchestrator";
    }
    }

    public async Task UnregisterAsync()
    {
        if (string.IsNullOrEmpty(_registrationId))
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
   await _httpClient.PostAsync($"{apiUrl}/api/agents/{_registrationId}/unregister", null);
     _logger.LogInformation("Agent unregistered successfully");
  }
   catch (HttpRequestException ex)
     {
   _logger.LogWarning(ex, "Network error during unregister (non-critical)");
 }
    catch (TaskCanceledException ex)
   {
           _logger.LogWarning(ex, "Unregister request timed out (non-critical)");
        }

  _registrationId = null;
        }
        catch (Exception ex)
        {
       _logger.LogError(ex, "Error unregistering agent");
     }
    finally
   {
            _semaphore.Release();
  }
    }

  public async Task StartHeartbeatAsync(CancellationToken cancellationToken)
 {
      if (string.IsNullOrEmpty(_registrationId))
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
   await _httpClient.PostAsync($"{apiUrl}/api/agents/{_registrationId}/heartbeat", null, cancellationToken);
       _logger.LogDebug("Heartbeat sent for agent {Id}", _registrationId);
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

     if (!string.IsNullOrEmpty(_registrationId))
   {
   consecutiveFailures = 0;
 await UpdateStatusAsync("Idle", cancellationToken);
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
   if (string.IsNullOrEmpty(_registrationId))
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

            await _httpClient.PostAsync($"{apiUrl}/api/agents/{_registrationId}/status", content, cancellationToken);
 _logger.LogInformation("Agent status updated to {Status}", status);
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
        _logger.LogInformation("Agent shutting down gracefully...");

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

    private class OrchestratorInfo
    {
        public string Id { get; set; } = "";
 public string Url { get; set; } = "";
    }

    public override void Dispose()
    {
   _httpClient?.Dispose();
        _semaphore?.Dispose();
        base.Dispose();
    }
}
