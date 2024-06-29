using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.WebSockets;
using System.Text;

namespace FinancialInstrumentsService.Tests
{
    public class WebSocketTests : IClassFixture<WebApplicationFactory<FinancialInstrumentService.Startup>>
    {
        private readonly WebApplicationFactory<FinancialInstrumentService.Startup> _factory;

        public WebSocketTests(WebApplicationFactory<FinancialInstrumentService.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WebSocket_CanConnectAndReceiveMessages()
        {
            // Arrange
            var client = _factory.Server.CreateWebSocketClient();
            var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws/btcusdt"), CancellationToken.None);

            // Act
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Assert
            Assert.Equal(WebSocketMessageType.Text, receiveResult.MessageType);
            Assert.False(receiveResult.EndOfMessage);

            var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            Assert.Contains("price", message);

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }

        [Fact]
        public async Task WebSocket_MultipleClientsCanSubscribe()
        {
            // Arrange
            var client = _factory.Server.CreateWebSocketClient();
            var webSocket1 = await client.ConnectAsync(new Uri("ws://localhost/ws/btcusdt"), CancellationToken.None);
            var webSocket2 = await client.ConnectAsync(new Uri("ws://localhost/ws/btcusdt"), CancellationToken.None);

            // Act
            var buffer1 = new byte[1024 * 4];
            var buffer2 = new byte[1024 * 4];

            var receiveResult1 = await webSocket1.ReceiveAsync(new ArraySegment<byte>(buffer1), CancellationToken.None);
            var receiveResult2 = await webSocket2.ReceiveAsync(new ArraySegment<byte>(buffer2), CancellationToken.None);

            // Assert
            Assert.Equal(WebSocketMessageType.Text, receiveResult1.MessageType);
            Assert.Equal(WebSocketMessageType.Text, receiveResult2.MessageType);

            var message1 = Encoding.UTF8.GetString(buffer1, 0, receiveResult1.Count);
            var message2 = Encoding.UTF8.GetString(buffer2, 0, receiveResult2.Count);

            Assert.Contains("price", message1);
            Assert.Contains("price", message2);

            await webSocket1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            await webSocket2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }
}
