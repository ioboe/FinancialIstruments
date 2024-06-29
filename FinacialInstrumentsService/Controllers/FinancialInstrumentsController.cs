using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FinancialInstrumentsController : ControllerBase
{
    private static readonly string[] instruments = new[] { "btcusdt", "ethusdt", "xrpusdt" };
    private readonly HttpClient _httpClient;

    public FinancialInstrumentsController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("instruments")]
    public IActionResult GetInstruments()
    {
        return Ok(instruments);
    }

    [HttpGet("price/{instrument}")]
    public async Task<IActionResult> GetPrice(string instrument)
    {
        if (!Array.Exists(instruments, i => i == instrument.ToLower()))
        {
            return NotFound(new { message = "Instrument not found" });
        }

        var response = await _httpClient.GetFromJsonAsync<object>($"https://api.binance.com/api/v3/ticker/price?symbol={instrument.ToUpper()}");
        return Ok(response);
    }
}
