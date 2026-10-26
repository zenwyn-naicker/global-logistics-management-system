namespace GLMS.Web.ViewModels;

public enum ServiceRequestStatusViewModel
{
    Open,
    InProgress,
    Completed,
    Cancelled
}

public class ServiceRequestViewModel
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal CostZar { get; set; }
    public ServiceRequestStatusViewModel Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
