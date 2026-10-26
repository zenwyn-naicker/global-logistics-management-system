namespace GLMS.Web.ViewModels;

public enum ContractStatusViewModel
{
    Draft,
    Active,
    Expired,
    OnHold,
    Declined
}

public class ContractViewModel
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ContractStatusViewModel Status { get; set; }
    public string ServiceLevel { get; set; } = string.Empty;
    public string? SignedAgreementPath { get; set; }
    public int ServiceRequestCount { get; set; }
    public List<ServiceRequestViewModel> ServiceRequests { get; set; } = new();

    public bool HasAgreement => !string.IsNullOrWhiteSpace(SignedAgreementPath);
}
