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
 * Author: ExchangeRate-API
 * Date: 2026
 * Title: Pair Conversion Requests
 * Type: [Source Code]
 * Available at: <https://www.exchangerate-api.com/docs/pair-conversion-requests> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Api.Dtos;
using GLMS.Api.Services.Strategies;

namespace GLMS.Api.Services;

public class CurrencyService : ICurrencyService
{
    private readonly CurrencyConversionService _currencyConversion;

    public CurrencyService(CurrencyConversionService currencyConversion)
    {
        _currencyConversion = currencyConversion;
    }

    public async Task<decimal> ConvertUsdToZarAsync(decimal amountUsd)
    {
        return await _currencyConversion.ConvertToZar(amountUsd);
    }

    public async Task<CurrencyRateDto> GetUsdToZarRateAsync()
    {
        var rate = await _currencyConversion.ConvertToZar(1m);

        return new CurrencyRateDto
        {
            Rate = rate,
            Strategy = _currencyConversion.GetActiveStrategy(),
            RetrievedAtUtc = DateTime.UtcNow
        };
    }

    public async Task<CurrencyPreviewDto> PreviewUsdToZarAsync(decimal amountUsd)
    {
        var amountZar = await _currencyConversion.ConvertToZar(amountUsd);

        return new CurrencyPreviewDto
        {
            AmountUsd = amountUsd,
            AmountZar = amountZar,
            Strategy = _currencyConversion.GetActiveStrategy()
        };
    }
}
