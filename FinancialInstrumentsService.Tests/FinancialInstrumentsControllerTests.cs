using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialInstrumentsService.Tests
{
    public class FinancialInstrumentsControllerTests : IClassFixture<WebApplicationFactory<FinancialInstrumentService.Startup>>
    {
        private readonly HttpClient _client;

        public FinancialInstrumentsControllerTests(WebApplicationFactory<FinancialInstrumentService.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetInstruments_ReturnsListOfInstruments()
        {
            // Act
            var response = await _client.GetAsync("/api/financialinstruments/instruments");

            // Assert
            response.EnsureSuccessStatusCode();
            var instruments = await response.Content.ReadAsStringAsync();
            Assert.Contains("btcusdt", instruments);
            Assert.Contains("ethusdt", instruments);
            Assert.Contains("xrpusdt", instruments);
        }

        [Fact]
        public async Task GetPrice_ValidInstrument_ReturnsPrice()
        {
            // Act
            var response = await _client.GetAsync("/api/financialinstruments/price/btcusdt");

            // Assert
            response.EnsureSuccessStatusCode();
            var price = await response.Content.ReadAsStringAsync();
            Assert.Contains("price", price); // Example check, adjust according to actual response structure
        }

        [Fact]
        public async Task GetPrice_InvalidInstrument_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/financialinstruments/price/invalid");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }

}
