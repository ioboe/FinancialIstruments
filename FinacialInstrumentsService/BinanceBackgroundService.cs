using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialInstrumentService
{
    public class BinanceBackgroundService : BackgroundService
    {
        private readonly WebSocketManager _webSocketManager;
        private readonly ILogger<BinanceBackgroundService> _logger;
        private readonly string[] instruments = new[] { "btcusdt", "ethusdt", "xrpusdt" };

        public BinanceBackgroundService(WebSocketManager webSocketManager, ILogger<BinanceBackgroundService> logger)
        {
            _webSocketManager = webSocketManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var instrument in instruments)
            {
                _ = Task.Run(() => ConnectToBinance(instrument, stoppingToken));
            }
        }

        private async Task ConnectToBinance(string instrument, CancellationToken stoppingToken)
        {
            var uri = new Uri($"wss://stream.binance.com:443/ws/{instrument}@aggTrade");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var client = new ClientWebSocket();
                await client.ConnectAsync(uri, stoppingToken);

                var buffer = new byte[1024 * 4];

                while (client.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
                {
                    var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                    var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);

                    await _webSocketManager.BroadcastMessageAsync(instrument, message);
                }
            }
        }
    }
}
