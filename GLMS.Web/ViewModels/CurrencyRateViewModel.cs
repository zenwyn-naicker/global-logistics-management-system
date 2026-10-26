namespace GLMS.Web.ViewModels;

public class CurrencyRateViewModel
{
    public string FromCurrency { get; set; } = "USD";
    public string ToCurrency { get; set; } = "ZAR";
    public decimal Rate { get; set; }
    public string Strategy { get; set; } = string.Empty;
    public DateTime RetrievedAtUtc { get; set; }
}
