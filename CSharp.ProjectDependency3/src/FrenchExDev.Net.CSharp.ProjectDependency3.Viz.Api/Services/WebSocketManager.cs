using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Services;

public class RegistryWebSocketManager
{
    private readonly List<WebSocket> _sockets = new();
    private readonly ILogger<RegistryWebSocketManager> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public RegistryWebSocketManager(ILogger<RegistryWebSocketManager> logger)
    {
        _logger = logger;
    }

    public async Task AddSocketAsync(WebSocket socket)
    {
        await _semaphore.WaitAsync();
        try
        {
            _sockets.Add(socket);
            _logger.LogInformation("WebSocket client connected. Total: {Count}", _sockets.Count);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task RemoveSocketAsync(WebSocket socket)
    {
        await _semaphore.WaitAsync();
        try
        {
            _sockets.Remove(socket);
            _logger.LogInformation("WebSocket client disconnected. Total: {Count}", _sockets.Count);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task BroadcastAsync(object message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        var buffer = new ArraySegment<byte>(bytes);

        await _semaphore.WaitAsync();
        try
        {
            var tasks = _sockets
                .Where(s => s.State == WebSocketState.Open)
                .Select(s => s.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None));

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting WebSocket message");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        await AddSocketAsync(webSocket);
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebSocket error");
        }
        finally
        {
            await RemoveSocketAsync(webSocket);
        }
    }
}
