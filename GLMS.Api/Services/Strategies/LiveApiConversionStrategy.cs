/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Refactoring.Guru
 * Date: 2026
 * Title: Strategy in C#
 * Type: [Source Code]
 * Available at: <https://refactoring.guru/design-patterns/strategy/csharp/example> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Anthropic
 * Date: 2026
 * Title: Sample code of Strategy pattern for MVC web app
 * Type: [Source Code]
 * Link to chat: <https://claude.ai/share/d67b55f8-8902-42d6-9787-94b14994261c> [Accessed 12 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Use the IHttpClientFactory
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2024
 * Title: How to use a JSON document object model in System.Text.Json
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: ExchangeRate-API
 * Date: 2026
 * Title: Pair Conversion Requests
 * Type: [Source Code]
 * Available at: <https://www.exchangerate-api.com/docs/pair-conversion-requests> [Accessed 6 April 2026]
 **************************************************************************************************************/

using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace GLMS.Api.Services.Strategies;

public class LiveApiConversionStrategy : ICurrencyConversionStrategy
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public LiveApiConversionStrategy(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ExchangeRateApi:ApiKey"] ?? "";
    }

    public bool IsAvailable()
    {
        return !string.IsNullOrWhiteSpace(_apiKey);
    }

    public async Task<decimal> Convert(decimal amountInUsd)
    {
        var amount = amountInUsd.ToString(CultureInfo.InvariantCulture);

        var response = await _httpClient.GetAsync(
            $"https://v6.exchangerate-api.com/v6/{_apiKey}/pair/USD/ZAR/{amount}");

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);

        if (document.RootElement.TryGetProperty("conversion_result", out var conversionResult))
            return Math.Round(conversionResult.GetDecimal(), 2);

        var rate = document.RootElement.GetProperty("conversion_rate").GetDecimal();
        return Math.Round(amountInUsd * rate, 2);
    }
}
