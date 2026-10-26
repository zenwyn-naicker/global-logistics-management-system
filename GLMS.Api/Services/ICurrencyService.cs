using GLMS.Api.Dtos;

namespace GLMS.Api.Services;

public interface ICurrencyService
{
    Task<decimal> ConvertUsdToZarAsync(decimal amountUsd);
    Task<CurrencyRateDto> GetUsdToZarRateAsync();
    Task<CurrencyPreviewDto> PreviewUsdToZarAsync(decimal amountUsd);
}
