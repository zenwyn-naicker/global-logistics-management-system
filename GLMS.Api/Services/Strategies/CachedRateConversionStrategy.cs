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

namespace GLMS.Api.Services.Strategies;

public class CachedRateConversionStrategy : ICurrencyConversionStrategy
{
    private static decimal? _cachedRate;
    private static DateTime _cacheExpiry = DateTime.MinValue;
    private readonly LiveApiConversionStrategy _liveStrategy;

    public CachedRateConversionStrategy(LiveApiConversionStrategy liveStrategy)
    {
        _liveStrategy = liveStrategy;
    }

    public bool IsAvailable()
    {
        return _cachedRate.HasValue && DateTime.UtcNow < _cacheExpiry;
    }

    public Task<decimal> Convert(decimal amountInUsd)
    {
        if (!_cachedRate.HasValue)
            throw new InvalidOperationException("No cached rate available.");

        return Task.FromResult(Math.Round(amountInUsd * _cachedRate.Value, 2));
    }

    public void UpdateCache(decimal rate)
    {
        _cachedRate = rate;
        _cacheExpiry = DateTime.UtcNow.AddHours(1);
    }
}
