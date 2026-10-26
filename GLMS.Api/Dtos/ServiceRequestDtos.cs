using System.ComponentModel.DataAnnotations;
using GLMS.Api.Models;

namespace GLMS.Api.Dtos;

public class ServiceRequestDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal CostZar { get; set; }
    public ServiceRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateServiceRequestDto
{
    [Range(1, int.MaxValue)]
    public int ContractId { get; set; }

    [Required, StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal CostUsd { get; set; }
}
