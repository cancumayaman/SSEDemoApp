using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/sse")]
public class SSEController : ControllerBase
{
    [HttpGet("price-stream")]
    public async Task PriceStream()
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("X-Accel-Buffering", "no");

        var rnd = new Random();
        var symbols = new[] { "BTC", "ETH", "SOL" };
        var prices = new Dictionary<string, decimal>
        {
            { "BTC", 60000m }, { "ETH", 3000m }, { "SOL", 150m }
        };
        var messageId = 0;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            await Response.WriteAsync("event: connected\n");
            await Response.WriteAsync("data: {\"status\":\"Bağlantı kuruldu, kripto fiyat güncellemeleri başlıyor\"}\n\n");
            await Response.Body.FlushAsync();

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                messageId++;

                var symbol = symbols[rnd.Next(symbols.Length)];
                var changePercent = (decimal)(rnd.NextDouble() - 0.5) * 2; // -1% … +1%
                var newPrice = prices[symbol] * (1 + changePercent / 100);
                prices[symbol] = Math.Round(newPrice, 2);

                var payload = new
                {
                    id = messageId,
                    timestamp = DateTime.UtcNow,
                    symbol,
                    newPrice = prices[symbol],
                    changePercent = Math.Round(changePercent, 2)
                };

                await Response.WriteAsync($"id: {messageId}\n");
                await Response.WriteAsync("event: price-update\n");
                await Response.WriteAsync($"data: {JsonSerializer.Serialize(payload, jsonOptions)}\n\n");
                await Response.Body.FlushAsync();

                await Task.Delay(rnd.Next(2000, 4000), HttpContext.RequestAborted);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SSE error: {ex}");
        }
        finally
        {
            Console.WriteLine("SSE connection ended.");
        }
    }
}
