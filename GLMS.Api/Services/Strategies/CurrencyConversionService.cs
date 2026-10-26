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

public class CurrencyConversionService
{
    private readonly List<ICurrencyConversionStrategy> _strategies;

    public CurrencyConversionService(
        LiveApiConversionStrategy liveStrategy,
        CachedRateConversionStrategy cachedStrategy,
        FixedRateConversionStrategy fixedStrategy)
    {
        _strategies = new List<ICurrencyConversionStrategy>
        {
            liveStrategy,
            cachedStrategy,
            fixedStrategy
        };
    }

    public async Task<decimal> ConvertToZar(decimal amountInUsd)
    {
        foreach (var strategy in _strategies)
        {
            if (!strategy.IsAvailable())
                continue;

            try
            {
                return await strategy.Convert(amountInUsd);
            }
            catch
            {
                continue;
            }
        }

        throw new InvalidOperationException("No currency conversion strategy available.");
    }

    public string GetActiveStrategy()
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.IsAvailable())
                return strategy.GetType().Name;
        }

        return "None";
    }
}
