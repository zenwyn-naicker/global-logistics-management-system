namespace GLMS.Api.Dtos;

public class CurrencyRateDto
{
    public string FromCurrency { get; set; } = "USD";
    public string ToCurrency { get; set; } = "ZAR";
    public decimal Rate { get; set; }
    public string Strategy { get; set; } = string.Empty;
    public DateTime RetrievedAtUtc { get; set; } = DateTime.UtcNow;
}

public class CurrencyPreviewDto
{
    public decimal AmountUsd { get; set; }
    public decimal AmountZar { get; set; }
    public string Strategy { get; set; } = string.Empty;
}
