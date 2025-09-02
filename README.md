# SSEDemoApp — Server-Sent Events (SSE) Crypto Price Stream

Minimal ASP.NET Core app that streams simulated crypto prices via SSE.

- Endpoint: GET /api/sse/price-stream
- Symbols: BTC, ETH, SOL
- Events: connected (once), price-update (every ~2–4s)

## Quick start
- Requirements: .NET 9 SDK, Visual Studio 2022
- Run in Visual Studio: open solution, set SSEDemoApp as startup, use __Debug > Start Debugging__
