using System.Collections.Concurrent;
using System.Net.WebSockets;

public class WebSocketManager
{
    private readonly ILogger<WebSocketManager> _logger;
    private readonly ConcurrentDictionary<string, ConcurrentBag<WebSocket>> _subscribers;

    public WebSocketManager(ILogger<WebSocketManager> logger)
    {
        _logger = logger;
        _subscribers = new ConcurrentDictionary<string, ConcurrentBag<WebSocket>>();
    }

    public async Task HandleWebSocketAsync(HttpContext context, string instrument)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        _logger.LogInformation($"Client connected for {instrument}");

        var subscribers = _subscribers.GetOrAdd(instrument, _ => new ConcurrentBag<WebSocket>());
        subscribers.Add(webSocket);

        await ReceiveMessages(webSocket, instrument);
    }

    private async Task ReceiveMessages(WebSocket webSocket, string instrument)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        _subscribers[instrument].TryTake(out _);
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        _logger.LogInformation($"Client disconnected from {instrument}");
    }

    public async Task BroadcastMessageAsync(string instrument, string message)
    {
        message = FormatMessage(message);
        if (_subscribers.TryGetValue(instrument, out var subscribers))
        {
            foreach (var socket in subscribers)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }

    private static string FormatMessage(string message)
    {
        return message.Replace("\"p\":", "\"price\":").Replace("\"s\":", "\"symbol\":");
    }
}
