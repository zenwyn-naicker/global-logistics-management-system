using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels;

public class CreateServiceRequestViewModel
{
    [Range(1, int.MaxValue)]
    public int ContractId { get; set; }

    [Required, StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal CostUsd { get; set; }
}
