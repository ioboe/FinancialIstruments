namespace FinancialInstrumentService.WebSocket
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketManager _webSocketManager;

        public WebSocketMiddleware(RequestDelegate next, WebSocketManager webSocketManager)
        {
            _next = next;
            _webSocketManager = webSocketManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/ws"))
            {
                var instrument = context.Request.Path.Value?.Split("/")[2];
                if (instrument == null)
                    return;

                await _webSocketManager.HandleWebSocketAsync(context, instrument);
            }
            else
            {
                await _next(context);
            }
        }
    }

}
